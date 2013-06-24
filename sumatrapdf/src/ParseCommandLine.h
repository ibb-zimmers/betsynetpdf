/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#ifndef ParseCommandLine_h
#define ParseCommandLine_h

#include "DisplayState.h"

class CommandLineInfo {
public:
    WStrVec     fileNames;
    // pathsToBenchmark contain 2 strings per each file to benchmark:
    // - name of the file to benchmark
    // - optional (NULL if not available) string that represents which pages
    //   to benchmark. It can also be a string "loadonly" which means we'll
    //   only benchmark loading of the catalog
    WStrVec     pathsToBenchmark;
    bool        makeDefault;
    bool        exitWhenDone;
    bool        printDialog;
    WCHAR *     printerName;
    WCHAR *     printSettings;
    COLORREF    bgColor;
    WCHAR *     inverseSearchCmdLine;
    WCHAR *     forwardSearchOrigin;
    int         forwardSearchLine;
    ForwardSearch forwardSearch;
    bool        escToExit;
    bool        reuseInstance;
    char *      lang;
    WCHAR *     destName;
    int         pageNumber;
    bool        restrictedUse;
    COLORREF    colorRange[2]; // foreground color / background color
    bool        enterPresentation;
    bool        enterFullscreen;
    DisplayMode startView;
    float       startZoom;
    PointI      startScroll;
    bool        showConsole;
    HWND        hwndPluginParent;
    WCHAR *     pluginURL;
    bool        exitImmediately;
    bool        silent;
    bool        cbxMangaMode;

    // stress-testing related
    WCHAR *     stressTestPath;
    WCHAR *     stressTestFilter; // NULL is equivalent to "*" (i.e. all files)
    WCHAR *     stressTestRanges;
    int         stressTestCycles;
    int         stressParallelCount;
    bool        stressRandomizeFiles;

    bool        crashOnOpen;

    CommandLineInfo() : makeDefault(false), exitWhenDone(false), printDialog(false),
        printerName(NULL), printSettings(NULL), bgColor((COLORREF)-1),
        escToExit(false), reuseInstance(false), lang(NULL),
        destName(NULL), pageNumber(-1), inverseSearchCmdLine(NULL),
        restrictedUse(false), pluginURL(NULL),
        enterPresentation(false), enterFullscreen(false), hwndPluginParent(NULL),
        startView(DM_AUTOMATIC), startZoom(INVALID_ZOOM), startScroll(PointI(-1, -1)),
        showConsole(false), exitImmediately(false), silent(false), cbxMangaMode(false),
        forwardSearchOrigin(NULL), forwardSearchLine(0),
        stressTestPath(NULL), stressTestFilter(NULL),
        stressTestRanges(NULL), stressTestCycles(1), stressParallelCount(1),
        stressRandomizeFiles(false),
        crashOnOpen(false)
    {
        colorRange[0] = RGB(0, 0, 0); // black
        colorRange[1] = RGB(0xFF, 0xFF, 0xFF); // white
        forwardSearch.highlightOffset = 0;
        forwardSearch.highlightWidth = 0;
        forwardSearch.highlightColor = (COLORREF)-1;
        forwardSearch.highlightPermanent = false;
    }

    ~CommandLineInfo() {
        free(printerName);
        free(printSettings);
        free(inverseSearchCmdLine);
        free(forwardSearchOrigin);
        free(lang);
        free(destName);
        free(stressTestPath);
        free(stressTestRanges);
        free(stressTestFilter);
        free(pluginURL);
    }

    void ParseCommandLine(WCHAR *cmdLine);
};

#endif
