/* Copyright 2014 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#ifndef Print_h
#define Print_h

class WindowInfo;
class BaseEngine;

bool PrintFile(const WCHAR *fileName, WCHAR *printerName=NULL, bool displayErrors=true, const WCHAR *settings=NULL);
bool PrintFile(BaseEngine *engine, WCHAR *printerName=NULL, bool displayErrors=true, const WCHAR *settings=NULL);
void OnMenuPrint(WindowInfo *win, bool waitForCompletion=false, bool printOnPlotter = false);
void AbortPrinting(WindowInfo *win);

#endif
