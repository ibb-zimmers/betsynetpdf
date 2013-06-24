/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: Simplified BSD (see COPYING.BSD) */

#ifndef Mui_h
#define Mui_h

// as little of mui as necessary to make ../EngineDump.cpp compile

using namespace Gdiplus;

namespace mui {

Font *GetCachedFont(const WCHAR *name, float size, FontStyle style);
Graphics *AllocGraphicsForMeasureText();
void FreeGraphicsForMeasureText(Graphics *g);

};

#endif
