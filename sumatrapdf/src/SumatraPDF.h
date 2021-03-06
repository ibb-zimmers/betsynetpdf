/* Copyright 2014 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#ifndef SumatraPDF_h
#define SumatraPDF_h

#include "DisplayState.h"

///////////////////////////////////////////////////////////////////////////////
// BetsyNetPDF
///////////////////////////////////////////////////////////////////////////////
#include <string>
#include <vector>
using namespace Gdiplus;
#include "GdiPlusUtil.h"
///////////////////////////////////////////////////////////////////////////////
// END BetsyNetPDF
///////////////////////////////////////////////////////////////////////////////

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

class Favorites;
class FileHistory;
class WindowInfo;
struct LabelWithCloseWnd;
struct TabData;

// all defined in SumatraPDF.cpp
extern bool                     gDebugShowLinks;
extern bool                     gShowFrameRate;
extern bool                     gUseGdiRenderer;
extern WCHAR *                  gPluginURL;
extern Vec<WindowInfo*>         gWindows;
extern Favorites                gFavorites;
extern FileHistory              gFileHistory;
extern WNDPROC                  DefWndProcCloseButton;

#define gPluginMode             (gPluginURL != NULL)

bool  HasPermission(int permission);
bool  IsUIRightToLeft();
bool  LaunchBrowser(const WCHAR *url);
bool  OpenFileExternally(const WCHAR *path);
void  AssociateExeWithPdfExtension();
void  CloseTab(WindowInfo *win, bool quitIfLast=false);
void  CloseWindow(WindowInfo *win, bool quitIfLast, bool forceClose=false);
void  SetSidebarVisibility(WindowInfo *win, bool tocVisible, bool favVisible);
void  RememberFavTreeExpansionState(WindowInfo *win);
void  LayoutTreeContainer(LabelWithCloseWnd *l, HWND hwndTree);
void  AdvanceFocus(WindowInfo* win);
bool  WindowInfoStillValid(WindowInfo *win);
void  SetCurrentLanguageAndRefreshUi(const char *langCode);
void  ShowOrHideToolbarGlobally();
void  ShowOrHideToolbarForWindow(WindowInfo *win);
void  UpdateDocumentColors();
void  UpdateCurrentFileDisplayStateForWin(WindowInfo *win);
void  UpdateTabFileDisplayStateForWin(WindowInfo *win, TabData *td);
bool  FrameOnKeydown(WindowInfo* win, WPARAM key, LPARAM lparam, bool inTextfield=false);
void  SwitchToDisplayMode(WindowInfo *win, DisplayMode displayMode, bool keepContinuous=false);
void  ReloadDocument(WindowInfo *win, bool autorefresh=false);
bool  CanSendAsEmailAttachment(WindowInfo *win=NULL);

COLORREF GetLogoBgColor();
COLORREF GetAboutBgColor();
COLORREF GetNoDocBgColor();

WindowInfo* FindWindowInfoByHwnd(HWND hwnd);
// note: background tabs are only searched if focusTab is true
WindowInfo* FindWindowInfoByFile(const WCHAR *file, bool focusTab);
WindowInfo* FindWindowInfoBySyncFile(const WCHAR *file, bool focusTab);

// LoadDocument carries a lot of state, this holds them in
// one place
struct LoadArgs
{
    explicit LoadArgs(const WCHAR *fileName, WindowInfo *win=NULL) :
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
WindowInfo *CreateAndShowWindowInfo();

UINT MbRtlReadingMaybe();
void MessageBoxWarning(HWND hwnd, const WCHAR *msg, const WCHAR *title = NULL);

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

class SubObject
{
public:
	double angle;

	SubObject(double x, double y, double dx, double dy, double angle);
	void SetDimensions(double x, double y, double dx, double dy);
	void Paint(Graphics* g, WindowInfo* win, RectF oobox, Color ooColor);
	std::string ToString();
	static SubObject* CreateFromString(std::string sobj);

private:
	double x_dpi, y_dpi, dx_dpi, dy_dpi;
};

class OverlayObject
{
public:
	std::string id, label, font;
	double angle, labelAngle;
	int page;
	float fontSize;
	bool selected, bold, italic, moveAll;
	GraphicsPath currentPath, currentLabelPath;
	Color foreGround, backGround;

	OverlayObject(std::string id, std::string label, std::string font, double x, double y, double dx, double dy, double lx, double ly, double rx, double ry, double angle, double labelAngle, float fontSize, Color foreGround, Color backGround, std::string subObjects);
	void Clone(OverlayObject* oo);
	void CreateSubObjects(std::string subObjects);
	double GetX();
	double GetY();
	double GetDX();
	double GetDY();
	double GetLX();
	double GetLY();
	double GetRX();
	double GetRY();
	void SetDimensions(double x, double y, double dx, double dy, double lx, double ly, double rx, double ry);
	void Move(double deltaX, double deltaY, bool moveLabel);
	void Paint(Graphics* g, WindowInfo* win, int pageNo, Region* bounds, Region* objRegion, Matrix* rotation, Matrix* elemRotation);
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
	double x_dpi, y_dpi, dx_dpi, dy_dpi, lx_dpi, ly_dpi, rx_dpi, ry_dpi;
	std::vector<SubObject*> subObjects; 

	void DrawLabelLine(Graphics* g, PointF* startPoints, PointF* endPoints, float zoom);
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
typedef void (__stdcall *OnMouseClickDelegate)(double x, double y, char* key);
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
	bool hitLabelForDragging, selectionChanging, mouseOverEnabled, measureMode, lineMode, useExternContextMenu, preventOverlayObjectSelection, showOverlapping, hideLabels, transparantOverlayObjects;
	std::string hwnd;
	std::string file;
	double fixAngle;
	OnMouseClickDelegate notifyMouseClick;
	PointD* lineStart;
	PointD* lineEnd;
	Point* curLineEnd;
	//Region objectsRegion, labelsRegion;

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
	void Escape(WindowInfo* win);

	//void ProcessOverlayObject(WindowInfo* win, char* id, char* label, char* font, double x, double y, double dx, double dy, double angle, float fontSize, Color foreGround, Color backGround, bool update, bool repaint = true);
	void ProcessOverlayObjects(WindowInfo* win, char* objects);
	void RemoveOverlayObject(WindowInfo* win, char* id);
	void SetSelectedOverlayObjects(WindowInfo* win, char* objectIds);
	bool CheckSelectionChanged(WindowInfo* win, bool ctrlPressed);
	char* GetSelectedOverlayObjectIds();
	char* GetSelectedOverlayObjects();
	void DeselectOverlayObjects();
	char* GetAllOverlayObjects();
	char* GetOverlayObjectAtPosition(WindowInfo* win, double x, double y);
	// x,y coords on screen
	void CheckMouseClick(WindowInfo* win, int x, int y, bool ctrlPressed);
	void CheckDeleteOverlayObject();
	void CheckOverlayObjectAtMousePos(WindowInfo* win, int x, int y, bool ctrlPressed, bool moveObj = true);
	void ClearOverlayObjectList(WindowInfo* win);
	void MoveSelectedOverlayObjectsBy(WindowInfo* win, int x, int y);
	void CheckOverlayObjectMoved(WindowInfo* win, int x, int y);

	void CheckOnRequestContextMenu(WindowInfo* win, int x, int y);

	static std::string GetFakedCmd(std::string file, std::string hwnd, bool directPrinting, bool defaultPrinter, std::string printerName);

private:
	PointD dragStart;
	PointD lastDragLoc;
	bool moveLabel;
	OnSelectionChangedDelegate notifySelectionChanged;
	OnDeleteDelegate notifyDelete;
	OnObjectMovedDelegate notifyObjectMoved;
	OnMouseOverObject notifyMouseOverObject;
	OnDistanceMeasuredDelegate notifyDistanceMeasured;
	OnLineDrawnDelegate notifyLineDrawn;
	OnRequestContextMenuDelegate requestContextMenu;

	void PutSelectedOverlayObjectsOnTop();
	OverlayObject* RemoveSelectedOverlayObject();

	void InitViewer();
};

#endif //__BetsyNetPDFUnmanagedApi_h__

#ifndef __BetsyNetPDFUnmanagedApiCallers_h__
#define __BetsyNetPDFUnmanagedApiCallers_h__

extern "C" {
	
	extern UNMANAGED_API WindowInfo* __stdcall CallBetsyNetPDFViewer(char* hwnd, char* file, bool useExternContextMenu, bool directPrinting, bool defaultPrinter, char* printerName, WindowInfo* win,
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
	extern UNMANAGED_API void __stdcall CallSetFixedAngle(WindowInfo* win, double angle);
	extern UNMANAGED_API void __stdcall CallSetDeactivateTextSelection(WindowInfo* win, bool value);
	extern UNMANAGED_API void __stdcall CallSetPreventOverlayObjectSelection(WindowInfo* win, bool value);
	extern UNMANAGED_API void __stdcall CallSetShowOverlapping(WindowInfo* win, bool value);
	extern UNMANAGED_API void __stdcall CallSetHideLabels(WindowInfo* win, bool value);
	extern UNMANAGED_API void __stdcall CallSetTransparantOverlayObjects(WindowInfo* win, bool value);
	extern UNMANAGED_API void __stdcall CallSetLineStart(WindowInfo* win, double x, double y);
	extern UNMANAGED_API PointF* __stdcall CallGetLineStart(WindowInfo* win);
	extern UNMANAGED_API bool __stdcall CallIsLineStart(WindowInfo* win);
	extern UNMANAGED_API PointF* __stdcall CallCvtScreen2Doc(WindowInfo* win, Point* screenCoords);
	extern UNMANAGED_API Point* __stdcall CallCvtDoc2Screen(WindowInfo* win, PointF* docCoords);

	//extern UNMANAGED_API void __stdcall CallProcessOverlayObject(WindowInfo* win, char* id, char* label, char* font, double x, double y, double dx, double dy, double angle, float fontSize, int foreGround, int backGround, bool update);
	extern UNMANAGED_API void __stdcall CallProcessOverlayObjects(WindowInfo* win, char* objs);
	extern UNMANAGED_API void __stdcall CallSetSelectedOverlayObjects(WindowInfo* win, char* objectIds);
	extern UNMANAGED_API void __stdcall CallRemoveOverlayObject(WindowInfo* win, char* id);
	extern UNMANAGED_API char* __stdcall CallGetSelectedOverlayObjectIds(WindowInfo* win);
	extern UNMANAGED_API char* __stdcall CallGetSelectedOverlayObjects(WindowInfo* win);
	extern UNMANAGED_API char* __stdcall CallGetAllOverlayObjects(WindowInfo* win);
	extern UNMANAGED_API char* __stdcall CallGetOverlayObjectAtPosition(WindowInfo* win, double x, double y);
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
	extern UNMANAGED_API int __stdcall CallGetDocumentRotation(WindowInfo* win);
}

#endif // __BetsyNetPDFUnmanagedApiCallers_h__

///////////////////////////////////////////////////////////////////////////////
// end BetsyNetPDF
///////////////////////////////////////////////////////////////////////////////

#endif
