/* Copyright 2014 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#ifndef SumatraAbout_h
#define SumatraAbout_h

#include "FileHistory.h"
#include "WindowInfo.h" // for StaticLinkInfo

void OnMenuAbout();

void  DrawAboutPage(WindowInfo& win, HDC hdc);

const WCHAR *GetStaticLink(Vec<StaticLinkInfo>& linkInfo, int x, int y, StaticLinkInfo *info=NULL);

#define SLINK_OPEN_FILE L"<File,Open>"
#define SLINK_LIST_SHOW L"<View,ShowList>"
#define SLINK_LIST_HIDE L"<View,HideList>"

void    DrawStartPage(WindowInfo& win, HDC hdc, FileHistory& fileHistory, COLORREF textColor, COLORREF backgroundColor);

#endif
