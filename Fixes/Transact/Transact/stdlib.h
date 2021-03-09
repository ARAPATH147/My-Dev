#ifndef _SIZE_T_DEFINED
#define _SIZE_T_DEFINED
typedef unsigned int size_t;
#endif

#ifndef _WCHAR_T_DEFINED
#define _WCHAR_T_DEFINED
typedef char wchar_t;
#endif

#ifndef NULL
#define NULL ((void *)0)
#endif

/* String conversions */
extern double atof(const char *nptr);
extern int atoi(const char *nptr);
extern long atol(const char *nptr);
extern double strtod(const char *nptr, char **endptr);
extern long strtol(const char *nptr, char **endptr, int base);
extern unsigned long strtoul(const char *nptr, char **endptr, int base);

/* Random number generators */

#define RAND_MAX (sizeof(int) == 2 ? 32767 : 2147483647)
#define MB_CUR_MAX      1

extern int rand(void);
extern void srand(unsigned int seed);

/* Memory management (Heap) functions */
extern void *calloc(unsigned int nelem, size_t elsize);
extern void free(void *ptr);
extern void *malloc(size_t size);
extern void *realloc(void *ptr, size_t size);

#if __HIGHC__   /* I.e., not ANSI */
/* Allocating objects > 64K in size (8086/286 only): */
_huge void * _halloc(long Elements, unsigned int Esize);
void _hfree(_huge void * v);
#endif

/* Communication with the environment */
extern void abort(void);
extern void exit(int status);
extern int atexit(void (*func)(void));
#define EXIT_FAILURE (-1)
#define EXIT_SUCCESS 0
extern char *getenv(const char *name);
extern int system(char *string);
long int labs(long int);

#ifndef _DIV_T_DEFINED
#define _DIV_T_DEFINED
typedef struct { int quot; int rem; } div_t;
typedef struct { long quot; long rem; } ldiv_t;
#endif

ldiv_t ldiv(long int numerator, long int denominator);
div_t div(int numerator, long int denominator);

void qsort(void *base, size_t nmemb, size_t size,
		int (*compar) (const void*, const void*));
void *bsearch(const void *key, const void *base,
	size_t nmemb, size_t size,
	int (*compar) (const void *, const void *));

/* Multibyte character functions */
extern  int 		mblen(const char *s, size_t n);
extern  int             mbtowc(wchar_t *pwc, const char *s, size_t n);
extern  int             wctomb(char *s, wchar_t wchar);

/* Multibyte string functions */
extern  size_t          mbstowcs(wchar_t *pwcs, const char *s, size_t n);
extern  size_t          wcstombs(char *s, const wchar_t *pwcs, size_t n);

/* onexit has been removed from the standard and replaced with atexit. */
/* We will eventually remove onexit, so modify your programs to use atexit. */
typedef void (*(*onexit_t)(void) )(void);
/* want to say: typedef onexit_t (*onexit_t) (void); */
extern onexit_t onexit(onexit_t (*func)(void));
	
