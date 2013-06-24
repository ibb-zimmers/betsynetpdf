/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: GPLv3 */

#include "BaseUtil.h"
#include "uia/DocumentProvider.h"

#include "DisplayModel.h"
#include "FileUtil.h"
#include "uia/Constants.h"
#include "uia/PageProvider.h"
#include "uia/Provider.h"
#include "uia/TextRange.h"

SumatraUIAutomationDocumentProvider::SumatraUIAutomationDocumentProvider(HWND canvasHwnd, SumatraUIAutomationProvider* root) :
    refCount(1), canvasHwnd(canvasHwnd), root(root),
    released(true), child_first(NULL), child_last(NULL),
    dm(NULL)
{
    //root->AddRef(); Don't add refs to our parent & owner. 
}

SumatraUIAutomationDocumentProvider::~SumatraUIAutomationDocumentProvider()
{
    this->FreeDocument();
}

void SumatraUIAutomationDocumentProvider::LoadDocument(DisplayModel* newDm)
{
    this->FreeDocument();

    // no mutexes needed, this function is called from thread that created dm

    // create page element for each page
    SumatraUIAutomationPageProvider* prevPage = NULL;
    for (int i=1; i<=newDm->PageCount(); ++i) {
        SumatraUIAutomationPageProvider* currentPage = new SumatraUIAutomationPageProvider(i, canvasHwnd, newDm, this);
        currentPage->sibling_prev = prevPage;
        if (prevPage)
            prevPage->sibling_next = currentPage;
        prevPage = currentPage;

        if (i == 1)
            child_first = currentPage;
    }
    child_last = prevPage;

    dm = newDm;
    released = false;
}

void SumatraUIAutomationDocumentProvider::FreeDocument()
{
    // release our refs to the page elements
    if (released)
        return;

    released = true;
    dm = NULL;

    SumatraUIAutomationPageProvider* it = child_first;
    while (it) {
        SumatraUIAutomationPageProvider* current = it;
        it = it->sibling_next;

        current->released = true; // disallow DisplayModel access
        current->Release();
    }

    // we have released our refs from these objects
    // we are not allowed to access them anymore
    child_first = NULL;
    child_last = NULL;
}

bool SumatraUIAutomationDocumentProvider::IsDocumentLoaded() const
{
    return !released;
}

DisplayModel* SumatraUIAutomationDocumentProvider::GetDM()
{
    AssertCrash(IsDocumentLoaded());
    AssertCrash(dm);
    return dm;
}

SumatraUIAutomationPageProvider* SumatraUIAutomationDocumentProvider::GetFirstPage()
{
    AssertCrash(IsDocumentLoaded());
    return child_first;
}

SumatraUIAutomationPageProvider* SumatraUIAutomationDocumentProvider::GetLastPage()
{
    AssertCrash(IsDocumentLoaded());
    return child_last;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::QueryInterface(const IID &iid,void **ppvObject)
{
    if (ppvObject == NULL)
        return E_POINTER;

    // TODO: per http://blogs.msdn.com/b/oldnewthing/archive/2004/03/26/96777.aspx should
    // respond to IUnknown
    if (iid == __uuidof(IRawElementProviderFragment)) {
        *ppvObject = static_cast<IRawElementProviderFragment*>(this);
        this->AddRef(); //New copy has entered the universe
        return S_OK;
    } else if (iid == __uuidof(IRawElementProviderSimple)) {
        *ppvObject = static_cast<IRawElementProviderSimple*>(this);
        this->AddRef(); //New copy has entered the universe
        return S_OK;
    } else if (iid == __uuidof(ITextProvider)) {
        *ppvObject = static_cast<ITextProvider*>(this);
        this->AddRef(); //New copy has entered the universe
        return S_OK;
    } else if (iid == IID_IAccIdentity) {
        *ppvObject = static_cast<IAccIdentity*>(this);
        this->AddRef(); //New copy has entered the universe
        return S_OK;
    }
    
    *ppvObject = NULL;
    return E_NOINTERFACE;
}

ULONG STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::AddRef(void)
{
    return InterlockedIncrement(&refCount);
}

ULONG STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::Release(void)
{
    LONG res = InterlockedDecrement(&refCount);
    CrashIf(res < 0);
    if (0 == res) {
        delete this;
    }
    return res;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::Navigate(enum NavigateDirection direction, IRawElementProviderFragment **pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    
    if (direction == NavigateDirection_NextSibling ||
        direction == NavigateDirection_PreviousSibling) {
        *pRetVal = NULL;
        return S_OK;
    } else if (direction == NavigateDirection_FirstChild ||
             direction == NavigateDirection_LastChild) {
        // don't allow traversion to enter invalid nodes
        if (released) {
            *pRetVal = NULL;
            return S_OK;
        }

        if (direction == NavigateDirection_FirstChild)
            *pRetVal = child_first;
        else
            *pRetVal = child_last;
        (*pRetVal)->AddRef();
        return S_OK;
    } else if (direction == NavigateDirection_Parent) {
        *pRetVal = root;
        (*pRetVal)->AddRef();
        return S_OK;
    } else {
        return E_INVALIDARG;
    }
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::GetRuntimeId(SAFEARRAY **pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;

    SAFEARRAY *psa = SafeArrayCreateVector(VT_I4, 0, 2);
    if (!psa)
        return E_OUTOFMEMORY;
    
    // RuntimeID magic, use hwnd to differentiate providers of different windows
    int rId[] = { (int)canvasHwnd, SUMATRA_UIA_DOCUMENT_RUNTIME_ID };
    for (LONG i = 0; i < 2; i++) {
        HRESULT hr = SafeArrayPutElement(psa, &i, (void*)&(rId[i]));
        CrashIf(FAILED(hr));
    }
    
    *pRetVal = psa;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::GetEmbeddedFragmentRoots(SAFEARRAY **pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;

    // no other roots => return NULL
    *pRetVal = NULL;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::SetFocus(void)
{
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::get_BoundingRectangle(struct UiaRect *pRetVal)
{
    // share area with the canvas uia provider
    return root->get_BoundingRectangle(pRetVal);
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::get_FragmentRoot(IRawElementProviderFragmentRoot **pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;

    // return the root node
    *pRetVal = root;
    root->AddRef();
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::GetPatternProvider(PATTERNID patternId,IUnknown **pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;

    if (patternId == UIA_TextPatternId) {
        *pRetVal = static_cast<ITextProvider*>(this);
        this->AddRef(); //New copy has entered the universe
        return S_OK;
    }

    *pRetVal = NULL;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::GetPropertyValue(PROPERTYID propertyId,VARIANT *pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    if (released)
        return E_FAIL;

    if (propertyId == UIA_NamePropertyId) {
        // typically filename
        pRetVal->vt = VT_BSTR;
        pRetVal->bstrVal = SysAllocString(path::GetBaseName(dm->FilePath()));
        return S_OK;
    } else if (propertyId == UIA_IsTextPatternAvailablePropertyId) {
        pRetVal->vt = VT_BOOL;
        pRetVal->boolVal = TRUE;
        return S_OK;
    } else if (propertyId == UIA_ControlTypePropertyId) {
        pRetVal->vt = VT_I4;
        pRetVal->lVal = UIA_DocumentControlTypeId;
        return S_OK;
    } else if (propertyId == UIA_IsContentElementPropertyId || propertyId == UIA_IsControlElementPropertyId) {
        pRetVal->vt = VT_BOOL;
        pRetVal->boolVal = TRUE;
        return S_OK;
    } else if (propertyId == UIA_NativeWindowHandlePropertyId) {
        pRetVal->vt = VT_I4;
        pRetVal->lVal = 0;
        return S_OK;
    } else if (propertyId == UIA_AutomationIdPropertyId) {
        pRetVal->vt = VT_BSTR;
        pRetVal->bstrVal = SysAllocString(L"Document");
        return S_OK;
    }

    pRetVal->vt = VT_EMPTY;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::get_HostRawElementProvider(IRawElementProviderSimple **pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    *pRetVal = NULL;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::get_ProviderOptions(ProviderOptions *pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    *pRetVal = ProviderOptions_ServerSideProvider;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::GetSelection(SAFEARRAY * *pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    if (released)
        return E_FAIL;

    SAFEARRAY *psa = SafeArrayCreateVector(VT_UNKNOWN, 0, 1);
    if (!psa)
        return E_OUTOFMEMORY;

    // TODO: this selection is leaked - why?
    SumatraUIAutomationTextRange* selection = new SumatraUIAutomationTextRange(this, dm->textSelection);

    LONG index = 0;
    HRESULT hr = SafeArrayPutElement(psa, &index, selection);
    CrashIf(FAILED(hr));
    // the array now owns the selection
    selection->Release();

    *pRetVal = psa;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::GetVisibleRanges(SAFEARRAY * *pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    if (released)
        return E_FAIL;

    // return all pages' ranges that are even partially visible
    Vec<SumatraUIAutomationTextRange*> rangeArray;
    SumatraUIAutomationPageProvider* it = child_first;
    while (it) {
        if (it->dm->GetPageInfo(it->pageNum) &&
            it->dm->GetPageInfo(it->pageNum)->shown &&
            it->dm->GetPageInfo(it->pageNum)->visibleRatio > 0.0f) {
            rangeArray.Append(new SumatraUIAutomationTextRange(this, it->pageNum));
        }
        it = it->sibling_next;
    }

    SAFEARRAY *psa = SafeArrayCreateVector(VT_UNKNOWN, 0, rangeArray.Size());
    if (!psa) {
        for (size_t i = 0; i < rangeArray.Size(); i++) {
            rangeArray[i]->Release();
        }
        return E_OUTOFMEMORY;
    }
    
    for (LONG i = 0; i < (LONG)rangeArray.Size(); i++) {
        HRESULT hr = SafeArrayPutElement(psa, &i, rangeArray[i]);
        CrashIf(FAILED(hr));
        rangeArray[i]->Release();
    }

    *pRetVal = psa;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::RangeFromChild(IRawElementProviderSimple *childElement, ITextRangeProvider **pRetVal)
{
    if (pRetVal == NULL || childElement == NULL)
        return E_POINTER;
    if (released)
        return E_FAIL;

    // get page range
    // TODO: is childElement guaranteed to be a SumatraUIAutomationPageProvider?
    *pRetVal = new SumatraUIAutomationTextRange(this, ((SumatraUIAutomationPageProvider*)childElement)->pageNum);
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::RangeFromPoint(struct UiaPoint point, ITextRangeProvider **pRetVal)
{
    // TODO: Is this even used? We wont support editing either way
    // so there won't be even a caret visible. Hence empty ranges are useless?
    return E_NOTIMPL;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::get_DocumentRange(ITextRangeProvider **pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    if (released)
        return E_FAIL;
    
    SumatraUIAutomationTextRange* documentRange = new SumatraUIAutomationTextRange(this);
    documentRange->SetToDocumentRange();

    *pRetVal = documentRange;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::get_SupportedTextSelection(enum SupportedTextSelection *pRetVal)
{
    if (pRetVal == NULL)
        return E_POINTER;
    *pRetVal = SupportedTextSelection_Single;
    return S_OK;
}

HRESULT STDMETHODCALLTYPE SumatraUIAutomationDocumentProvider::GetIdentityString(DWORD dwIDChild, BYTE **ppIDString, DWORD *pdwIDStringLen)
{
    if (ppIDString == NULL || pdwIDStringLen == NULL)
        return E_POINTER;
    if (released)
        return E_FAIL;

    for (SumatraUIAutomationPageProvider* it = child_first; it; it = it->sibling_next) {
        if (it->pageNum == (int)dwIDChild + 1) {
            // Use memory address as identification. Use 8 bytes just in case
            *ppIDString = (BYTE*) CoTaskMemAlloc(8);
            if (!(*ppIDString))
                return E_OUTOFMEMORY;

            memset(*ppIDString, 0, 8);
            memcpy(*ppIDString, &it, sizeof(void*)); // copy the pointer to the allocated array
            *pdwIDStringLen = 8;
            return S_OK;
        }
    }

    return E_FAIL;
}
