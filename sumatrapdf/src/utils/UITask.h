/* Copyright 2013 the SumatraPDF project authors (see AUTHORS file).
   License: Simplified BSD (see COPYING.BSD) */

#ifndef UITask_h
#define UITask_h

// Base class for code that has to be executed on UI thread. Derive your class
// from UITask and call uitask::Post() to schedule execution
// of its Execute() method on UI thread
class UITask
{
public:
    // for debugging
    const char *name;

    UITask() : name("UITask") {}
    virtual ~UITask() {}
    virtual void Execute() = 0;
};

typedef void (*UITaskFuncPtr)(void *arg);

namespace uitask {

// Call Initialize() at program startup and Destroy() at the end
void    Initialize();
void    Destroy();

// Can be called from any thread. Queues the task to be executed
// as soon as possible on ui thread.
void    Post(UITask *);

void    Post(UITaskFuncPtr, void *arg);
}

#endif
