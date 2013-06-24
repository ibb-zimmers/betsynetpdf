/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#ifndef EbookControls_h
#define EbookControls_h

#include "Doc.h"
#include "Mui.h"

class HtmlFormatter;
class HtmlFormatterArgs;
class PageControl;
using namespace mui;

// controls managed by EbookController
struct EbookControls {
    ParsedMui *         muiDef;
    HwndWrapper *       mainWnd;
    PageControl *       page;
    ScrollBar *         progress;
    Button *            status;
    ILayout *           topPart;
};

EbookControls * CreateEbookControls(HWND hwnd);
void            DestroyEbookControls(EbookControls* controls);
void            SetMainWndBgCol(EbookControls *ctrls);

class HtmlPage;

// control that shows a single ebook page
// TODO: move to a separate file
class PageControl : public Control
{
    HtmlPage *  page;
    int         cursorX, cursorY;

public:
    PageControl();

    void      SetPage(HtmlPage *newPage);
    HtmlPage* GetPage() const { return page; }

    Size GetDrawableSize();

    virtual void Paint(Graphics *gfx, int offX, int offY);

    virtual void NotifyMouseMove(int x, int y);
};

#endif
