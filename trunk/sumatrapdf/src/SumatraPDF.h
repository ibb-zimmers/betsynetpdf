/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#ifndef SumatraPDF_h
#define SumatraPDF_h

#include "FileHistory.h"
#include "Favorites.h"
#include "SumatraWindow.h"
#include "Translations.h"
#include "DisplayModel.h"
#include "ParseCommandLine.h"
#include <string>
#include <vector>
using namespace Gdiplus;
#include "GdiPlusUtil.h"

#define FRAME_CLASS_NAME        L"SUMATRA_PDF_FRAME"
#define SUMATRA_WINDOW_TITLE    L"SumatraPDF"

#define WEBSITE_MAIN_URL         L"http://blog.kowalczyk.info/software/sumatrapdf/"
#define WEBSITE_MANUAL_URL       L"http://blog.kowalczyk.info/software/sumatrapdf/manual.html"
#define WEBSITE_TRANSLATIONS_URL L"http://code.google.com/p/sumatrapdf/wiki/HelpTranslateSumatra"

#ifndef CRASH_REPORT_URL
#define CRASH_REPORT_URL         L"http://blog.kowalczyk.info/software/sumatrapdf/develop.html"
#endif

// permissions that can be revoked (or explicitly set) through Group Policies
enum {
    // enables Update checks, crash report submitting and hyperlinks
    Perm_InternetAccess     = 1 << 0,
    // enables opening and saving documents and launching external viewers
    Perm_DiskAccess         = 1 << 1,
    // enables persistence of preferences to disk (includes the Frequently Read page and Favorites)
    Perm_SavePreferences    = 1 << 2,
    // enables setting as default viewer
    Perm_RegistryAccess     = 1 << 3,
    // enables printing
    Perm_PrinterAccess      = 1 << 4,
    // enables image/text selections and selection copying (if permitted by the document)
    Perm_CopySelection      = 1 << 5,
    // enables fullscreen and presentation view modes
    Perm_FullscreenAccess   = 1 << 6,
    // enables all of the above
    Perm_All                = 0x0FFFFFF,
    // set through the command line (Policies might only apply when in restricted use mode)
    Perm_RestrictedUse      = 0x1000000,
};

enum MenuToolbarFlags {
    MF_NO_TRANSLATE      = 1 << 0,
    MF_PLUGIN_MODE_ONLY  = 1 << 1,
    MF_NOT_FOR_CHM       = 1 << 2,
    MF_NOT_FOR_EBOOK_UI  = 1 << 3,
    MF_CBX_ONLY          = 1 << 4,
#define PERM_FLAG_OFFSET 5
    MF_REQ_INET_ACCESS   = Perm_InternetAccess << PERM_FLAG_OFFSET,
    MF_REQ_DISK_ACCESS   = Perm_DiskAccess << PERM_FLAG_OFFSET,
    MF_REQ_PREF_ACCESS   = Perm_SavePreferences << PERM_FLAG_OFFSET,
    MF_REQ_PRINTER_ACCESS= Perm_PrinterAccess << PERM_FLAG_OFFSET,
    MF_REQ_ALLOW_COPY    = Perm_CopySelection << PERM_FLAG_OFFSET,
    MF_REQ_FULLSCREEN    = Perm_FullscreenAccess << PERM_FLAG_OFFSET,
};

/* styling for About/Properties windows */

#define LEFT_TXT_FONT           L"Arial"
#define LEFT_TXT_FONT_SIZE      12
#define RIGHT_TXT_FONT          L"Arial Black"
#define RIGHT_TXT_FONT_SIZE     12
// for backward compatibility use a value that older versions will render as yellow
#define ABOUT_BG_COLOR_DEFAULT  (RGB(0xff, 0xf2, 0) - 0x80000000)

class WindowInfo;
class EbookWindow;
class Favorites;

// all defined in SumatraPDF.cpp
extern HINSTANCE                ghinst;
extern bool                     gDebugShowLinks;
extern bool                     gUseGdiRenderer;
extern HCURSOR                  gCursorHand;
extern HCURSOR                  gCursorArrow;
extern HCURSOR                  gCursorIBeam;
extern HFONT                    gDefaultGuiFont;
extern WCHAR *                  gPluginURL;
extern Vec<WindowInfo*>         gWindows;
extern Vec<EbookWindow*>        gEbookWindows;
extern Favorites                gFavorites;
extern FileHistory              gFileHistory;
extern WNDPROC                  DefWndProcCloseButton;

#define gPluginMode             (gPluginURL != NULL)

LRESULT CALLBACK WndProcCloseButton(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam);
bool  HasPermission(int permission);
bool  IsUIRightToLeft();
bool  LaunchBrowser(const WCHAR *url);
bool  OpenFileExternally(const WCHAR *path);
void  AssociateExeWithPdfExtension();
void  CloseWindow(WindowInfo *win, bool quitIfLast, bool forceClose=false);
void  SetSidebarVisibility(WindowInfo *win, bool tocVisible, bool favVisible);
void  RememberFavTreeExpansionState(WindowInfo *win);
void  LayoutTreeContainer(HWND hwndContainer, int id);
void  AdvanceFocus(WindowInfo* win);
bool  WindowInfoStillValid(WindowInfo *win);
void  SetCurrentLanguageAndRefreshUi(const char *langCode);
void  ShowOrHideToolbarGlobally();
void  UpdateDocumentColors();
void  UpdateCurrentFileDisplayStateForWin(const SumatraWindow& win);
bool  FrameOnKeydown(WindowInfo* win, WPARAM key, LPARAM lparam, bool inTextfield=false);
void  SwitchToDisplayMode(WindowInfo *win, DisplayMode displayMode, bool keepContinuous=false);
void  ReloadDocument(WindowInfo *win, bool autorefresh=false);
bool  CanSendAsEmailAttachment(WindowInfo *win=NULL);
bool  DoCachePageRendering(WindowInfo *win, int pageNo);
void  OnMenuOptions(HWND hwnd);
void  OnMenuAdvancedOptions();
void  OnMenuExit();
void  AutoUpdateCheckAsync(HWND hwnd, bool autoCheck);
void  OnMenuChangeLanguage(HWND hwnd);
void  OnDropFiles(HDROP hDrop, bool dragFinish=true);
void  OnMenuOpen(const SumatraWindow& win);
size_t TotalWindowsCount();
void  CloseDocumentInWindow(WindowInfo *win);
void  CloseDocumentAndDeleteWindowInfo(WindowInfo *win);
void  OnMenuAbout();
void  QuitIfNoMoreWindows();
bool  ShouldSaveThumbnail(DisplayState& ds);
void  SaveThumbnailForFile(const WCHAR *filePath, RenderedBitmap *bmp);

COLORREF GetLogoBgColor();
COLORREF GetAboutBgColor();
COLORREF GetNoDocBgColor();

WindowInfo* FindWindowInfoByFile(const WCHAR *file);
WindowInfo* FindWindowInfoByHwnd(HWND hwnd);
WindowInfo* FindWindowInfoBySyncFile(const WCHAR *file);

// TODO: this is hopefully temporary
// LoadDocument carries a lot of state, this holds them in
// one place
struct LoadArgs
{
    LoadArgs(const WCHAR *fileName, WindowInfo *win=NULL) :
        fileName(fileName), win(win), showWin(true), forceReuse(false),
        isNewWindow(false), allowFailure(true), placeWindow(true) { }

    const WCHAR *fileName;
    WindowInfo *win;
    bool showWin;
    bool forceReuse;
    // for internal use
    bool isNewWindow;
    bool allowFailure;
    bool placeWindow;
};

WindowInfo* LoadDocument(LoadArgs& args);
void        LoadDocument2(const WCHAR *fileName, const SumatraWindow& win);
WindowInfo *CreateAndShowWindowInfo();

///////////////////////////////////////////////////////////////////////////////
// BetsyNetPDF
///////////////////////////////////////////////////////////////////////////////

// Put this class in your personal toolbox... 
 template<class E, 
 class T = std::char_traits<E>, 
 class A = std::allocator<E> > 
  
 class Widen : public std::unary_function< 
     const std::string&, std::basic_string<E, T, A> > 
 { 
     std::locale loc_; 
     const std::ctype<E>* pCType_; 
  
     // No copy-constructor, no assignment operator... 
     Widen(const Widen&); 
     Widen& operator= (const Widen&); 
  
 public: 
     // Constructor... 
     Widen(const std::locale& loc = std::locale()) : loc_(loc) 
     { 
#if defined(_MSC_VER) && (_MSC_VER < 1300) // VC++ 6.0... 
         using namespace std; 
         pCType_ = &_USE(loc, ctype<E> ); 
#else 
         pCType_ = &std::use_facet<std::ctype<E> >(loc); 
#endif 
     } 
  
     // Conversion... 
     std::basic_string<E, T, A> operator() (const std::string& str) const 
     { 
         typename std::basic_string<E, T, A>::size_type srcLen = 
             str.length(); 
         const char* pSrcBeg = str.c_str(); 
         std::vector<E> tmp(srcLen); 
  
         pCType_->widen(pSrcBeg, pSrcBeg + srcLen, &tmp[0]); 
         return std::basic_string<E, T, A>(&tmp[0], srcLen); 
     } 
 }; 

class OverlayObject
{
public:
	std::string id, label, font;
	double angle;
	int page;
	float fontSize;
	bool selected, bold, italic;
	std::vector<PointF> currentScreenLocation, currentLabelLocation;
	Color foreGround, backGround;

	OverlayObject(std::string id, std::string label, std::string font, double x, double y, double dx, double dy , double lx, double ly, double angle, float fontSize, Color foreGround, Color backGround);
	void Clone(OverlayObject* oo);
	double GetX();
	double GetY();
	double GetDX();
	double GetDY();
	double GetLX();
	double GetLY();
	void SetX(double x);
	void SetY(double y);
	void SetDX(double dx);
	void SetDY(double dy);
	void SetLX(double lx);
	void SetLY(double ly);
	void InitLXY(WindowInfo* win);
	void Move(double deltaX, double deltaY, bool moveLabel);
	void Paint(Graphics* g, WindowInfo* win, int pageNo, RectI bounds);
	void MakeVisible(WindowInfo* win);
	bool CheckSelectionChanged(WindowInfo* win);
	bool CheckIsInSelection(WindowInfo* win);
	bool CheckIsInSelection(WindowInfo* win, bool label);
	bool CheckIsPolyInSelection(WindowInfo* win, bool label);
	RectF GetRect(std::vector<PointF> points);
	std::string ToString();
	static OverlayObject* CreateFromString(std::string sobj);
	static bool CheckSegementIntersection(float p0_x, float p0_y, float p1_x, float p1_y, float p2_x, float p2_y, float p3_x, float p3_y);

private:
	double x_dpi, y_dpi, dx_dpi, dy_dpi, lx_dpi, ly_dpi;
};

#ifndef __BetsyNetPDFUnmanagedApi_h__
#define __BetsyNetPDFUnmanagedApi_h__

#define UNMANAGED_EXPORTS

#ifdef UNMANAGED_EXPORTS
#define UNMANAGED_API __declspec(dllexport)
#else
#define UNMANAGED_API __declspec(dllimport)
#endif

typedef void (__stdcall *OnSelectionChangedDelegate)();
typedef void (__stdcall *OnDeleteDelegate)();
typedef void (__stdcall *OnMouseOverObject)(char* id);
// x,y coords on page
typedef void (__stdcall *OnMouseClickDelegate)(double x, double y);
typedef void (__stdcall *OnObjectMovedDelegate)(double deltaX, double deltaY, bool moveLabel);
typedef void (__stdcall *OnDistanceMeasuredDelegate)(double distance);
typedef void (__stdcall *OnLineDrawnDelegate)(double p1x, double p1y, double p2x, double p2y);
//x,y coords on screen
typedef void (__stdcall *OnRequestContextMenuDelegate)(int x, int y, char* id);

class UNMANAGED_API BetsyNetPDFUnmanagedApi 
{
public:
	std::vector<OverlayObject*> overlayObjects;
	OverlayObject* lastObj;
	bool hitLabelForDragging, selectionChanging, mouseOverEnabled, measureMode, lineMode, useExternContextMenu;
	std::string hwnd;
	std::string file;

	BetsyNetPDFUnmanagedApi();
	void BetsyNetPDFViewer(char* hwnd, char* file);
	void UpdateViewer(WindowInfo* win, char* hwnd);
	void SetDelegates(
		OnSelectionChangedDelegate selChanged, 
		OnMouseClickDelegate mouseClick, 
		OnDeleteDelegate onDelete, 
		OnObjectMovedDelegate onMove, 
		OnRequestContextMenuDelegate requestContextMenuPointer, 
		OnMouseOverObject onMouseOver,
		OnDistanceMeasuredDelegate onMeasure,
		OnLineDrawnDelegate onLine);

	void DrawOverlayObjets(HDC* hdc, WindowInfo* win, int pageNo, RectI bounds);
	void DrawLine(HDC* hdc, WindowInfo* win);
	void SetCurrentLineEnd(WindowInfo* win, int x, int y);

	//void ProcessOverlayObject(WindowInfo* win, char* id, char* label, char* font, double x, double y, double dx, double dy, double angle, float fontSize, Color foreGround, Color backGround, bool update, bool repaint = true);
	void ProcessOverlayObjects(WindowInfo* win, char* objects);
	void RemoveOverlayObject(WindowInfo* win, char* id);
	void SetSelectedOverlayObjects(WindowInfo* win, char* objectIds);
	bool CheckSelectionChanged(WindowInfo* win, WPARAM key);
	char* GetSelectedOverlayObjectIds();
	char* GetSelectedOverlayObjects();
	void DeselectOverlayObjects();
	char* GetAllOverlayObjects();
	// x,y coords on screen
	void CheckMouseClick(WindowInfo* win, int x, int y, WPARAM key);
	void CheckDeleteOverlayObject();
	void CheckOverlayObjectAtMousePos(WindowInfo* win, int x, int y, bool ctrl, bool moveObj = true);
	void ClearOverlayObjectList(WindowInfo* win);
	void MoveSelectedOverlayObjectsBy(WindowInfo* win, int x, int y);
	void CheckOverlayObjectMoved(WindowInfo* win, int x, int y);

	void CheckOnRequestContextMenu(WindowInfo* win, int x, int y);

	static void GetFakedCmd(CommandLineInfo& i, std::string file, std::string hwnd);

private:
	PointD dragStart;
	PointD lastDragLoc;
	PointD* lineStart;
	PointD* lineEnd;
	Point* curLineEnd;
	bool moveLabel;
	OnSelectionChangedDelegate notifySelectionChanged;
	OnDeleteDelegate notifyDelete;
	OnMouseClickDelegate notifyMouseClick;
	OnObjectMovedDelegate notifyObjectMoved;
	OnMouseOverObject notifyMouseOverObject;
	OnDistanceMeasuredDelegate notifyDistanceMeasured;
	OnLineDrawnDelegate notifyLineDrawn;
	OnRequestContextMenuDelegate requestContextMenu;

	void InitViewer();
};

#endif //__BetsyNetPDFUnmanagedApi_h__

#ifndef __BetsyNetPDFUnmanagedApiCallers_h__
#define __BetsyNetPDFUnmanagedApiCallers_h__

extern "C" {
	
	extern UNMANAGED_API WindowInfo* __stdcall CallBetsyNetPDFViewer(char* hwnd, char* file, bool useExternContextMenu,
		OnSelectionChangedDelegate selChangedPtr, 
		OnMouseClickDelegate mouseClickPointer, 
		OnDeleteDelegate onDeletePointer, 
		OnObjectMovedDelegate onMovePointer, 
		OnRequestContextMenuDelegate requestContextMenuPointer,
		OnMouseOverObject onMouseOver,
		OnDistanceMeasuredDelegate onMeasure,
		OnLineDrawnDelegate onLine);
	extern UNMANAGED_API void __stdcall CallOpenNewFile(WindowInfo* win, char* file);
	extern UNMANAGED_API bool __stdcall CallIsDocOpen(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallUpdateViewer(WindowInfo* win, char* hwnd);
	extern UNMANAGED_API void __stdcall CallFocusViewer(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallSetMouseOverEnabled(WindowInfo* win, bool enabled);
	extern UNMANAGED_API void __stdcall CallSetMeasureModeEnabled(WindowInfo* win, bool enabled);
	extern UNMANAGED_API void __stdcall CallSetLineModeEnabled(WindowInfo* win, bool enabled);
	extern UNMANAGED_API void __stdcall CallSetDeactivateTextSelection(WindowInfo* win, bool value);
	extern UNMANAGED_API PointF* __stdcall CallCvtScreen2Doc(WindowInfo* win, Point* screenCoords);
	extern UNMANAGED_API Point* __stdcall CallCvtDoc2Screen(WindowInfo* win, PointF* docCoords);

	//extern UNMANAGED_API void __stdcall CallProcessOverlayObject(WindowInfo* win, char* id, char* label, char* font, double x, double y, double dx, double dy, double angle, float fontSize, int foreGround, int backGround, bool update);
	extern UNMANAGED_API void __stdcall CallProcessOverlayObjects(WindowInfo* win, char* objs);
	extern UNMANAGED_API void __stdcall CallSetSelectedOverlayObjects(WindowInfo* win, char* objectIds);
	extern UNMANAGED_API void __stdcall CallRemoveOverlayObject(WindowInfo* win, char* id);
	extern UNMANAGED_API char* __stdcall CallGetSelectedOverlayObjectIds(WindowInfo* win);
	extern UNMANAGED_API char* __stdcall CallGetSelectedOverlayObjects(WindowInfo* win);
	extern UNMANAGED_API char* __stdcall CallGetAllOverlayObjects(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallClearOverlayObjectList(WindowInfo* win);

	extern UNMANAGED_API void __stdcall CallSaveAs(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallPrint(WindowInfo* win, bool printOnPlotter);
	extern UNMANAGED_API void __stdcall CallFitPageWidth(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallFitWholePage(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallZoomOut(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallZoomIn(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallRotateLeft(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallRotateRight(WindowInfo* win);
	extern UNMANAGED_API void __stdcall CallRotateCounterClockWise(WindowInfo* win, int angle);
}

#endif // __BetsyNetPDFUnmanagedApiCallers_h__

///////////////////////////////////////////////////////////////////////////////
// end BetsyNetPDF
///////////////////////////////////////////////////////////////////////////////

#endif
