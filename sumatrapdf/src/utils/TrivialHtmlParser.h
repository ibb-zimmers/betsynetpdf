/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: Simplified BSD (see COPYING.BSD) */

#ifndef TrivialHtmlParser_h
#define TrivialHtmlParser_h

#include "HtmlParserLookup.h"

enum HtmlParseError {
    ErrParsingNoError,
    ErrParsingElement, // syntax error parsing element
    ErrParsingExclOrPI,
    ErrParsingClosingElement, // syntax error in closing element
    ErrParsingElementName, // syntax error after element name
    ErrParsingAttributes, // syntax error in attributes
    ErrParsingAttributeName, // syntax error after attribute name
    ErrParsingAttributeValue,
};

struct HtmlAttr;
struct HtmlToken;

struct HtmlElement {
    HtmlTag tag;
    char *name; // name is NULL whenever tag != Tag_NotFound
    HtmlAttr *firstAttr;
    HtmlElement *up, *down, *next;
    UINT codepage;

    bool NameIs(const char *name) const;
    bool NameIsNS(const char *name, const char *ns) const;

    WCHAR *GetAttribute(const char *name) const;
    HtmlElement *GetChildByTag(HtmlTag tag, int idx=0) const;
};

class HtmlParser {
    PoolAllocator allocator;

    // text to parse. It can be changed.
    char *html;
    // true if s was allocated by ourselves, false if managed
    // by the caller
    bool freeHtml;
    // the codepage used for converting text to Unicode
    UINT codepage;

    size_t elementsCount;
    size_t attributesCount;

    HtmlElement *rootElement;
    HtmlElement *currElement;

    HtmlElement *AllocElement(HtmlTag tag, char *name, HtmlElement *parent);
    HtmlAttr *AllocAttr(char *name, HtmlAttr *next);

    void CloseTag(HtmlToken *tok);
    void StartTag(HtmlToken *tok);
    void AppendAttr(char *name, char *value);

    HtmlElement *FindParent(HtmlToken *tok);
    HtmlElement *ParseError(HtmlParseError err) {
        error = err;
        return NULL;
    }

    void Reset();

public:
    HtmlParseError error;  // parsing error, a static string
    const char *errorContext; // pointer within html showing which part we failed to parse

    HtmlParser();
    ~HtmlParser();

    HtmlElement *Parse(const char *s, UINT codepage=CP_ACP);
    HtmlElement *ParseInPlace(char *s, UINT codepage=CP_ACP);

    size_t ElementsCount() const {
        return elementsCount;
    }

    size_t TotalAttrCount() const {
        return attributesCount;
    }

    HtmlElement *FindElementByName(const char *name, HtmlElement *from=NULL);
    HtmlElement *FindElementByNameNS(const char *name, const char *ns, HtmlElement *from=NULL);
};

WCHAR *DecodeHtmlEntitites(const char *string, UINT codepage);

#endif
