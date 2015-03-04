/* Copyright 2014 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#ifndef AppUtil_h
#define AppUtil_h

bool IsValidProgramVersion(const char *txt);
int CompareVersion(const WCHAR *txt1, const WCHAR *txt2);

bool AdjustVariableDriveLetter(WCHAR *path);

bool IsUntrustedFile(const WCHAR *filePath, const WCHAR *fileUrl=NULL);

#endif
