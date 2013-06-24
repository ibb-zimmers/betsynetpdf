/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#include "BaseUtil.h"
#include "StressTesting.h"

#include "AppPrefs.h"
#include "AppTools.h"
#include "DirIter.h"
#include "Doc.h"
#include "FileUtil.h"
#include "HtmlWindow.h"
#include "Notifications.h"
#include "ParseCommandLine.h"
#include "RenderCache.h"
#include "SimpleLog.h"
#include "Search.h"
#include "SumatraPDF.h"
#include "Timer.h"
#include "WindowInfo.h"
#include "WinUtil.h"

#define FIRST_STRESS_TIMER_ID 101

static slog::Logger *gLog;
#define logbench(msg, ...) gLog->LogFmt(TEXT(msg), __VA_ARGS__)

static bool gIsStressTesting = false;
static int gCurrStressTimerId = FIRST_STRESS_TIMER_ID;

bool IsStressTesting()
{
    return gIsStressTesting;
}

struct PageRange {
    PageRange() : start(1), end(INT_MAX) { }
    PageRange(int start, int end) : start(start), end(end) { }

    int start, end; // end == INT_MAX means to the last page
};

// parses a list of page ranges such as 1,3-5,7- (i..e all but pages 2 and 6)
// into an interable list (returns NULL on parsing errors)
// caller must delete the result
static bool ParsePageRanges(const WCHAR *ranges, Vec<PageRange>& result)
{
    if (!ranges)
        return false;

    WStrVec rangeList;
    rangeList.Split(ranges, L",", true);
    rangeList.SortNatural();

    for (size_t i = 0; i < rangeList.Count(); i++) {
        int start, end;
        if (str::Parse(rangeList.At(i), L"%d-%d%$", &start, &end) && 0 < start && start <= end)
            result.Append(PageRange(start, end));
        else if (str::Parse(rangeList.At(i), L"%d-%$", &start) && 0 < start)
            result.Append(PageRange(start, INT_MAX));
        else if (str::Parse(rangeList.At(i), L"%d%$", &start) && 0 < start)
            result.Append(PageRange(start, start));
        else
            return false;
    }

    return result.Count() > 0;
}

// a valid page range is a non-empty, comma separated list of either
// single page ("3") numbers, closed intervals "2-4" or intervals
// unlimited to the right ("5-")
bool IsValidPageRange(const WCHAR *ranges)
{
    Vec<PageRange> rangeList;
    return ParsePageRanges(ranges, rangeList);
}

inline bool IsInRange(Vec<PageRange>& ranges, int pageNo)
{
    for (size_t i = 0; i < ranges.Count(); i++) {
        if (ranges.At(i).start <= pageNo && pageNo <= ranges.At(i).end)
            return true;
    }
    return false;
}

static void BenchLoadRender(BaseEngine *engine, int pagenum)
{
    Timer t(true);
    bool ok = engine->BenchLoadPage(pagenum);
    t.Stop();

    if (!ok) {
        logbench("Error: failed to load page %d", pagenum);
        return;
    }
    double timems = t.GetTimeInMs();
    logbench("pageload   %3d: %.2f ms", pagenum, timems);

    t.Start();
    RenderedBitmap *rendered = engine->RenderBitmap(pagenum, 1.0, 0);
    t.Stop();

    if (!rendered) {
        logbench("Error: failed to render page %d", pagenum);
        return;
    }
    delete rendered;
    timems = t.GetTimeInMs();
    logbench("pagerender %3d: %.2f ms", pagenum, timems);
}

// <s> can be:
// * "loadonly"
// * description of page ranges e.g. "1", "1-5", "2-3,6,8-10"
bool IsBenchPagesInfo(const WCHAR *s)
{
    return str::EqI(s, L"loadonly") || IsValidPageRange(s);
}

static void BenchFile(WCHAR *filePath, const WCHAR *pagesSpec)
{
    if (!file::Exists(filePath)) {
        return;
    }

    Timer total(true);
    logbench("Starting: %s", filePath);

    Timer t(true);
    BaseEngine *engine = EngineManager::CreateEngine(filePath, gGlobalPrefs->chmUI.useFixedPageUI);
    t.Stop();

    if (!engine) {
        logbench("Error: failed to load %s", filePath);
        return;
    }

    double timems = t.GetTimeInMs();
    logbench("load: %.2f ms", timems);
    int pages = engine->PageCount();
    logbench("page count: %d", pages);

    if (NULL == pagesSpec) {
        for (int i = 1; i <= pages; i++) {
            BenchLoadRender(engine, i);
        }
    }

    assert(!pagesSpec || IsBenchPagesInfo(pagesSpec));
    Vec<PageRange> ranges;
    if (ParsePageRanges(pagesSpec, ranges)) {
        for (size_t i = 0; i < ranges.Count(); i++) {
            for (int j = ranges.At(i).start; j <= ranges.At(i).end; j++) {
                if (1 <= j && j <= pages)
                    BenchLoadRender(engine, j);
            }
        }
    }

    delete engine;
    total.Stop();

    logbench("Finished (in %.2f ms): %s", total.GetTimeInMs(), filePath);
}

static void BenchDir(WCHAR *dir)
{
    WStrVec files;
    ScopedMem<WCHAR> pattern(str::Format(L"%s\\*.pdf", dir));
    CollectPathsFromDirectory(pattern, files);
    for (size_t i = 0; i < files.Count(); i++) {
        BenchFile(files.At(i), NULL);
    }
}

void BenchFileOrDir(WStrVec& pathsToBench)
{
    gLog = new slog::StderrLogger();

    size_t n = pathsToBench.Count() / 2;
    for (size_t i = 0; i < n; i++) {
        WCHAR *path = pathsToBench.At(2 * i);
        if (file::Exists(path))
            BenchFile(path, pathsToBench.At(2 * i + 1));
        else if (dir::Exists(path))
            BenchDir(path);
        else
            logbench("Error: file or dir %s doesn't exist", path);
    }

    delete gLog;
}

inline bool IsSpecialDir(const WCHAR *s)
{
    return str::Eq(s, L".") || str::Eq(s, L"..");
}

static bool IsStressTestSupportedFile(const WCHAR *fileName, const WCHAR *filter)
{
    if (filter && !path::Match(fileName, filter))
        return false;
    return EngineManager::IsSupportedFile(fileName);
}

static bool CollectStressTestSupportedFilesFromDirectory(const WCHAR *dirPath, const WCHAR *filter, WStrVec& paths)
{
    ScopedMem<WCHAR> pattern(path::Join(dirPath, L"*"));

    WIN32_FIND_DATA fdata;
    HANDLE hfind = FindFirstFile(pattern, &fdata);
    if (INVALID_HANDLE_VALUE == hfind)
        return false;

    do {
        if (!(fdata.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
            if (IsStressTestSupportedFile(fdata.cFileName, filter)) {
                paths.Append(path::Join(dirPath, fdata.cFileName));
            }
        }
    } while (FindNextFile(hfind, &fdata));
    FindClose(hfind);

    return paths.Count() > 0;
}

// return t1 - t2 in seconds
static int SystemTimeDiffInSecs(SYSTEMTIME& t1, SYSTEMTIME& t2)
{
    FILETIME ft1, ft2;
    SystemTimeToFileTime(&t1, &ft1);
    SystemTimeToFileTime(&t2, &ft2);
    return FileTimeDiffInSecs(ft1, ft2);
}

static int SecsSinceSystemTime(SYSTEMTIME& time)
{
    SYSTEMTIME currTime;
    GetSystemTime(&currTime);
    return SystemTimeDiffInSecs(currTime, time);
}

static WCHAR *FormatTime(int totalSecs)
{
    int secs = totalSecs % 60;
    int totalMins = totalSecs / 60;
    int mins = totalMins % 60;
    int hrs = totalMins / 60;
    if (hrs > 0)
        return str::Format(L"%d hrs %d mins %d secs", hrs, mins, secs);
    if (mins > 0)
        return str::Format(L"%d mins %d secs", mins, secs);
    return str::Format(L"%d secs", secs);
}

static void FormatTime(int totalSecs, str::Str<char> *s)
{
    int secs = totalSecs % 60;
    int totalMins = totalSecs / 60;
    int mins = totalMins % 60;
    int hrs = totalMins / 60;
    if (hrs > 0)
        s->AppendFmt("%d hrs %d mins %d secs", hrs, mins, secs);
    if (mins > 0)
        s->AppendFmt("%d mins %d secs", mins, secs);
    s->AppendFmt("%d secs", secs);
}

static void MakeRandomSelection(DisplayModel *dm, int pageNo)
{
    if (!dm->ValidPageNo(pageNo))
        pageNo = 1;
    if (!dm->ValidPageNo(pageNo))
        return;

    // try a random position in the page
    int x = rand() % 640;
    int y = rand() % 480;
    if (dm->textSelection->IsOverGlyph(pageNo, x, y)) {
        dm->textSelection->StartAt(pageNo, x, y);
        dm->textSelection->SelectUpTo(pageNo, rand() % 640, rand() % 480);
    }
}

// encapsulates the logic of getting the next file to test, so
// that we can implement different strategies
class TestFileProvider {
public:
    virtual ~TestFileProvider() {}
    // returns path of the next file to test or NULL if done
    virtual WCHAR *NextFile() = 0;
    // start the iteration from the beginning
    virtual void Restart() = 0;
};

class FilesProvider : public TestFileProvider {
    WStrVec files;
    size_t provided;
public:
    FilesProvider(const WCHAR *path) {
        files.Append(str::Dup(path));
        provided = 0;
    }
    FilesProvider(WStrVec& newFiles, int n, int offset) {
        // get every n-th file starting at offset
        for (size_t i = offset; i < newFiles.Count(); i += n) {
            WCHAR *f = newFiles[i];
            files.Append(str::Dup(f));
        }
        provided = 0;
    }

    virtual ~FilesProvider() {}

    virtual WCHAR *NextFile() {
        if (provided >= files.Count())
            return NULL;
        return files[provided++];
    }

    virtual void Restart() {
        provided = 0;
    }
};

class DirFileProvider : public TestFileProvider {

    ScopedMem<WCHAR>  startDir;
    ScopedMem<WCHAR>  fileFilter;

    // current state of directory traversal
    WStrVec           filesToOpen;
    WStrVec           dirsToVisit;

    bool OpenDir(const WCHAR *dirPath);
public:
    DirFileProvider(const WCHAR *path, const WCHAR *filter);
    virtual ~DirFileProvider();
    virtual WCHAR *NextFile();
    virtual void Restart();
};

DirFileProvider::DirFileProvider(const WCHAR *path, const WCHAR *filter)
{
    startDir.Set(str::Dup(path));
    if (filter && !str::Eq(filter, L"*"))
        fileFilter.Set(str::Dup(filter));
    OpenDir(path);
}

DirFileProvider::~DirFileProvider()
{
}

bool DirFileProvider::OpenDir(const WCHAR *dirPath)
{
    assert(filesToOpen.Count() == 0);

    bool hasFiles = CollectStressTestSupportedFilesFromDirectory(dirPath, fileFilter, filesToOpen);
    filesToOpen.SortNatural();

    ScopedMem<WCHAR> pattern(str::Format(L"%s\\*", dirPath));
    bool hasSubDirs = CollectPathsFromDirectory(pattern, dirsToVisit, true);

    return hasFiles || hasSubDirs;
}

WCHAR *DirFileProvider::NextFile()
{
    while (filesToOpen.Count() > 0) {
        ScopedMem<WCHAR> path(filesToOpen.At(0));
        filesToOpen.RemoveAt(0);
        return path.StealData();
    }

    if (dirsToVisit.Count() > 0) {
        // test next directory
        ScopedMem<WCHAR> path(dirsToVisit.At(0));
        dirsToVisit.RemoveAt(0);
        OpenDir(path);
        return NextFile();
    }

    return NULL;
}

void DirFileProvider::Restart()
{
    OpenDir(startDir);
}

static size_t GetAllMatchingFiles(const WCHAR *dir, const WCHAR *filter, WStrVec& files, bool showProgress)
{
    WStrVec dirsToVisit;
    dirsToVisit.Append(str::Dup(dir));

    while (dirsToVisit.Count() > 0) {
        if (showProgress) {
            wprintf(L".");
            fflush(stdout);
        }

        ScopedMem<WCHAR> path(dirsToVisit[0]);
        dirsToVisit.RemoveAt(0);
        CollectStressTestSupportedFilesFromDirectory(path, filter, files);
        ScopedMem<WCHAR> pattern(str::Format(L"%s\\*", path));
        CollectPathsFromDirectory(pattern, dirsToVisit, true);
    }
    return files.Count();
}

/* The idea of StressTest is to render a lot of PDFs sequentially, simulating
a human advancing one page at a time. This is mostly to run through a large number
of PDFs before a release to make sure we're crash proof. */

class StressTest {
    WindowInfo *      win;
    RenderCache *     renderCache;
    Timer             currPageRenderTime;
    int               currPage;
    int               pageForSearchStart;
    int               filesCount; // number of files processed so far
    int               timerId;
    bool              exitWhenDone;

    SYSTEMTIME        stressStartTime;
    int               cycles;
    Vec<PageRange>    pageRanges;
    // range of files to render (files get a new index when going through several cycles)
    Vec<PageRange>    fileRanges;
    int               fileIndex;

    // owned by StressTest
    TestFileProvider *fileProvider;

    bool OpenFile(const WCHAR *fileName);

    bool GoToNextPage();
    bool GoToNextFile();

    void TickTimer();
    void Finished(bool success);

public:
    StressTest(WindowInfo *win, RenderCache *renderCache, bool exitWhenDone) :
        win(win), renderCache(renderCache), currPage(0), pageForSearchStart(0),
        filesCount(0), cycles(1), fileIndex(0), fileProvider(NULL),
        exitWhenDone(exitWhenDone)
    {
        timerId = gCurrStressTimerId++;
    }
    ~StressTest() {
        delete fileProvider;
    }

    void Start(const WCHAR *path, const WCHAR *filter, const WCHAR *ranges, int cycles);
    void Start(TestFileProvider *fileProvider, int cycles);

    void OnTimer(int timerIdGot);
    void GetLogInfo(str::Str<char> *s);
};

void StressTest::Start(TestFileProvider *fileProvider, int cycles)
{
    GetSystemTime(&stressStartTime);

    this->fileProvider = fileProvider;
    this->cycles = cycles;

    if (pageRanges.Count() == 0)
        pageRanges.Append(PageRange());
    if (fileRanges.Count() == 0)
        fileRanges.Append(PageRange());

    TickTimer();
}

void StressTest::Start(const WCHAR *path, const WCHAR *filter, const WCHAR *ranges, int cycles)
{
    if (file::Exists(path)) {
        FilesProvider *filesProvider = new FilesProvider(path);
        ParsePageRanges(ranges, pageRanges);
        Start(filesProvider, cycles);
    }
    else if (dir::Exists(path)) {
        DirFileProvider *dirFileProvider = new DirFileProvider(path, filter);
        ParsePageRanges(ranges, fileRanges);
        Start(dirFileProvider, cycles);
    }
    else {
        // Note: string dev only, don't translate
        ScopedMem<WCHAR> s(str::Format(L"Path '%s' doesn't exist", path));
        ShowNotification(win, s, false /* autoDismiss */, true, NG_STRESS_TEST_SUMMARY);
        Finished(false);
    }
}

void StressTest::Finished(bool success)
{
    win->stressTest = NULL; // make sure we're not double-deleted

    if (success) {
        int secs = SecsSinceSystemTime(stressStartTime);
        ScopedMem<WCHAR> tm(FormatTime(secs));
        ScopedMem<WCHAR> s(str::Format(L"Stress test complete, rendered %d files in %s", filesCount, tm));
        ShowNotification(win, s, false, false, NG_STRESS_TEST_SUMMARY);
    }

    CloseWindow(win, exitWhenDone);
    delete this;
}

bool StressTest::GoToNextFile()
{
    for (;;) {
        WCHAR *nextFile = fileProvider->NextFile();
        if (nextFile) {
            if (!IsInRange(fileRanges, ++fileIndex))
                continue;
            if (OpenFile(nextFile))
                return true;
            continue;
        }
        if (--cycles <= 0)
            return false;
        fileProvider->Restart();
    }
}

bool StressTest::OpenFile(const WCHAR *fileName)
{
    wprintf(L"%s\n", fileName);
    fflush(stdout);

    LoadArgs args(fileName);
    args.forceReuse = rand() % 3 != 1;
    WindowInfo *w = LoadDocument(args);
    if (!w)
        return false;

    if (w == win) { // WindowInfo reused
        if (!win->dm)
            return false;
    } else if (!w->dm) { // new WindowInfo
        CloseWindow(w, false);
        return false;
    }

    // transfer ownership of stressTest object to a new window and close the
    // current one
    assert(this == win->stressTest);
    if (w != win) {
        if (win->IsDocLoaded()) {
            // try to provoke a crash in RenderCache cleanup code
            ClientRect rect(win->hwndFrame);
            rect.Inflate(rand() % 10, rand() % 10);
            SendMessage(win->hwndFrame, WM_SIZE, 0, MAKELONG(rect.dx, rect.dy));
            win->RequestRendering(1);
            win->RepaintAsync();
        }

        WindowInfo *toClose = win;
        w->stressTest = win->stressTest;
        win->stressTest = NULL;
        win = w;
        CloseWindow(toClose, false);
    }
    if (!win->dm)
        return false;

    win->dm->ChangeDisplayMode(DM_CONTINUOUS);
    win->dm->ZoomTo(ZOOM_FIT_PAGE);
    win->dm->GoToFirstPage();
    if (win->tocVisible || gGlobalPrefs->showFavorites)
        SetSidebarVisibility(win, win->tocVisible, gGlobalPrefs->showFavorites);

    currPage = pageRanges.At(0).start;
    win->dm->GoToPage(currPage, 0);
    currPageRenderTime.Start();
    ++filesCount;

    pageForSearchStart = (rand() % win->dm->PageCount()) + 1;
    // search immediately in single page documents
    if (1 == pageForSearchStart) {
        // use text that is unlikely to be found, so that we search all pages
        win::SetText(win->hwndFindBox, L"!z_yt");
        FindTextOnThread(win);
    }

    int secs = SecsSinceSystemTime(stressStartTime);
    ScopedMem<WCHAR> tm(FormatTime(secs));
    ScopedMem<WCHAR> s(str::Format(L"File %d: %s, time: %s", filesCount, fileName, tm));
    ShowNotification(win, s, false, false, NG_STRESS_TEST_SUMMARY);

    return true;
}

bool StressTest::GoToNextPage()
{
    double pageRenderTime = currPageRenderTime.GetTimeInMs();
    ScopedMem<WCHAR> s(str::Format(L"Page %d rendered in %d milliseconds", currPage, (int)pageRenderTime));
    ShowNotification(win, s, true, false, NG_STRESS_TEST_BENCHMARK);

    ++currPage;
    while (!IsInRange(pageRanges, currPage) && currPage <= win->dm->PageCount()) {
        currPage++;
    }

    if (currPage > win->dm->PageCount()) {
        if (GoToNextFile())
            return true;
        Finished(true);
        return false;
    }

    win->dm->GoToPage(currPage, 0);
    currPageRenderTime.Start();

    // start text search when we're in the middle of the document, so that
    // search thread touches both pages that were already rendered and not yet
    // rendered
    // TODO: it would be nice to also randomize search starting page but the
    // current API doesn't make it easy
    if (currPage == pageForSearchStart) {
        // use text that is unlikely to be found, so that we search all pages
        win::SetText(win->hwndFindBox, L"!z_yt");
        FindTextOnThread(win);
    }

    if (1 == rand() % 3) {
        ClientRect rect(win->hwndFrame);
        int deltaX = (rand() % 40) - 23;
        rect.dx += deltaX;
        if (rect.dx < 300)
            rect.dx += (abs(deltaX) * 3);
        int deltaY = (rand() % 40) - 23;
        rect.dy += deltaY;
        if (rect.dy < 300)
            rect.dy += (abs(deltaY) * 3);
        SendMessage(win->hwndFrame, WM_SIZE, 0, MAKELONG(rect.dx, rect.dy));
    }
    return true;
}

void StressTest::TickTimer()
{
    SetTimer(win->hwndFrame, timerId, USER_TIMER_MINIMUM, NULL);
}

void StressTest::OnTimer(int timerIdGot)
{
    CrashIf(timerId != timerIdGot);
    KillTimer(win->hwndFrame, timerId);
    if (!win->dm || !win->dm->engine) {
        if (!GoToNextFile()) {
            Finished(true);
            return;
        }
        TickTimer();
        return;
    }

    // chm documents aren't rendered and we block until we show them
    // so we can assume previous page has been shown and go to next page
    if (win->dm->AsChmEngine()) {
        if (!GoToNextPage())
            return;
        goto Next;
    }

    // For non-image files, we detect if a page was rendered by checking the cache
    // (but we don't wait more than 3 seconds).
    // Image files are always fully rendered in WM_PAINT, so we know the page
    // has already been rendered.
    bool didRender = renderCache->Exists(win->dm, currPage, win->dm->Rotation());
    if (!didRender && DoCachePageRendering(win, currPage)) {
        double timeInMs = currPageRenderTime.GetTimeInMs();
        if (timeInMs > 3.0 * 1000) {
            if (!GoToNextPage())
                return;
        }
    } else {
        if (!GoToNextPage())
            return;
    }
Next:
    MakeRandomSelection(win->dm, currPage);
    TickTimer();
}

// note: used from CrashHandler, shouldn't allocate memory
void StressTest::GetLogInfo(str::Str<char> *s)
{
    s->AppendFmt(", stress test rendered %d files in ", filesCount);
    FormatTime(SecsSinceSystemTime(stressStartTime), s);
    s->AppendFmt(", currPage: %d", currPage);
}

// note: used from CrashHandler.cpp, should not allocate memory
void GetStressTestInfo(str::Str<char>* s)
{
    // only add paths to files encountered during an explicit stress test
    // (for privacy reasons, users should be able to decide themselves
    // whether they want to share what files they had opened during a crash)
    if (!IsStressTesting())
        return;

    for (size_t i = 0; i < gWindows.Count(); i++) {
        WindowInfo *w = gWindows.At(i);
        if (!w || !w->dm || !w->loadedFilePath)
            continue;

        s->Append("File: ");
        char buf[256];
        str::conv::ToCodePageBuf(buf, dimof(buf), w->loadedFilePath, CP_UTF8);
        s->Append(buf);
        w->stressTest->GetLogInfo(s);
        s->Append("\r\n");
    }
}

enum { MaxTypes = 32 };
struct FilesPerType {
    WCHAR *ext; // without the '.'
    WStrVec *filesAll;
    WStrVec *filesRandom;
};

// Select random files to test. We want to test each file type equally, so
// we first group them by file extension and then select up to maxPerType
// for each extension, randomly, and inter-leave the files with different
// extensions, so their testing is evenly distributed.
// Returns result in <files>.
static void RandomizeFiles(WStrVec& files, int maxPerType)
{
    int nTypes = 0;
    FilesPerType filesPerType[MaxTypes];

    for (size_t i=0; i < files.Count(); i++) {
        WCHAR *file = files.At(i);
        const WCHAR *ext = str::FindCharLast(file, L'.');
        CrashAlwaysIf(!ext);
        ext++;
        int typeNo = -1;
        for (int j=0; j<nTypes; j++) {
            if (str::EqI(ext, filesPerType[j].ext)) {
                typeNo = j;
                break;
            }
        }
        if (-1 == typeNo) {
            typeNo = nTypes++;
            CrashAlwaysIf(nTypes >= MaxTypes);
            filesPerType[typeNo].ext = str::Dup(ext);
            filesPerType[typeNo].filesAll = new WStrVec();
            filesPerType[typeNo].filesRandom = NULL;
        }
        filesPerType[typeNo].filesAll->Append(str::Dup(file));
    }

    for (int j=0; j<nTypes; j++) {
        WStrVec *all = filesPerType[j].filesAll;
        WStrVec *random = new WStrVec();
        filesPerType[j].filesRandom = random;

        for (int n=0; n < maxPerType && all->Count() > 0; n++) {
            int idx = rand() % all->Count();
            WCHAR *file = all->At(idx);
            random->Append(file);
            all->RemoveAtFast(idx);
        }
    }

    files.Reset();
    bool gotAll = false;
    while (!gotAll) {
        gotAll = true;
        for (int j=0; j<nTypes; j++) {
            WStrVec *random = filesPerType[j].filesRandom;
            if (random->Count() > 0) {
                gotAll = false;
                WCHAR *file = random->At(0);
                files.Append(file);
                random->RemoveAtFast(0);
            }
        }
    }

    for (int j=0; j<nTypes; j++) {
        free(filesPerType[j].ext);
        delete filesPerType[j].filesAll;
        delete filesPerType[j].filesRandom;
    }
}

void StartStressTest(CommandLineInfo *i, WindowInfo *win, RenderCache *renderCache)
{
    // gPredictiveRender = false;
    gIsStressTesting = true;
    // TODO: for now stress testing only supports the non-ebook ui
    gGlobalPrefs->ebookUI.useFixedPageUI = gGlobalPrefs->chmUI.useFixedPageUI = true;
    // forbid entering sleep mode during tests
    SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
    srand((unsigned int)time(NULL));
    // redirect stderr to NUL to disable (MuPDF) logging
    FILE *nul; freopen_s(&nul, "NUL", "w", stderr);

    int n = i->stressParallelCount;
    if (n > 1 || i->stressRandomizeFiles) {
        WindowInfo **windows = AllocArray<WindowInfo*>(n);
        windows[0] = win;
        for (int j=1; j<n; j++) {
            windows[j] = CreateAndShowWindowInfo();
            if (!windows[j])
                return;
        }
        WStrVec filesToTest;

        wprintf(L"Scanning for files in directory %s\n", i->stressTestPath);
        fflush(stdout);
        size_t filesCount = GetAllMatchingFiles(i->stressTestPath, i->stressTestFilter, filesToTest, true);
        if (0 == filesCount) {
            wprintf(L"Didn't find any files matching filter '%s'\n", i->stressTestFilter);
            return;
        }
        wprintf(L"Found %d files", (int)filesCount);
        fflush(stdout);
        if (i->stressRandomizeFiles) {
            // TODO: should probably allow over-writing the 100 limit
            RandomizeFiles(filesToTest, 100);
            filesCount = filesToTest.Count();
            wprintf(L"\nAfter randomization: %d files", (int)filesCount);
        }
        wprintf(L"\n");
        fflush(stdout);

        for (int j=0; j<n; j++) {
            // dst will be deleted when the stress ends
            win = windows[j];
            StressTest *dst = new StressTest(win, renderCache, i->exitWhenDone);
            win->stressTest = dst;
            // divide filesToTest among each window
            FilesProvider *filesProvider = new FilesProvider(filesToTest, n, j);
            dst->Start(filesProvider, i->stressTestCycles);
        }

        free(windows);
    } else {
        // dst will be deleted when the stress ends
        StressTest *dst = new StressTest(win, renderCache, i->exitWhenDone);
        win->stressTest = dst;
        dst->Start(i->stressTestPath, i->stressTestFilter, i->stressTestRanges, i->stressTestCycles);
    }
}

void OnStressTestTimer(WindowInfo *win, int timerId)
{
    win->stressTest->OnTimer(timerId);
}

void FinishStressTest(WindowInfo *win)
{
    delete win->stressTest;
}
