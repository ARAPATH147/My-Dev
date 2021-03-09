/* string.h */
#ifndef _SIZE_T_DEFINED
#define _SIZE_T_DEFINED
typedef unsigned int size_t;
#endif
#define _MAXSTRING (sizeof(int) == 2 ? 65535 : 4294967295)

extern void *memchr(void *s, int c, size_t n);

#undef memcmp
extern int memcmp(const void *s1, const void *s2, size_t n);
#if __HIGHC__  
#define memcmp(s1, s2, n) (_compare(s1, s2, n))
#endif

extern void *memcpy(void *s1, const void *s2, size_t n);
extern void *memmove(void *s1, const void *s2, size_t n);
extern void *memset(void *s, int c, size_t n);

#undef strcat
extern char *strcat(char *s1, const char *s2);
#if 0   
/* USE THIS MACRO FOR SPEED 
 * only if you don't mind multiple evaluation of the arguments. 
*/
#define strcat(s1, s2) \
  (_move(s2, ((char *)s1)+_find_char(s1, _MAXSTRING, 0), _find_char(s2,_MAXSTRING,0)+1), s1)
#endif

extern char *strchr(const char *s, int c);

#undef strcmp
extern int strcmp(const char *s1, const char *s2);
#define strcmp(s1, s2) (strncmp(s1,s2,_MAXSTRING))

#undef strcpy
extern char *strcpy(char *s1, const char *s2);
#if 0   
/* USE THIS MACRO FOR SPEED 
 * only if you don't mind multiple evaluation of the arguments. 
*/
#define strcpy(s1, s2) (_move(s2, s1, _find_char(s2, _MAXSTRING, 0)+1), s1)
#endif

extern size_t strcspn(char *s1, char *s2);

extern size_t strlen(const char *s);

extern char *strncat(char *s1, const char*s2, size_t n);

extern int strncmp(const char *s1, const char *s2, size_t n);

extern char *strncpy(char *s1, const char *s2, size_t n);

extern char *strpbrk(const char *s1, const char *s2);

extern char *strrchr(const char *s, int c);

extern size_t strspn(const char *s1, const char *s2);

extern char *strtok(char *s1, const char *s2);

extern char *_cat_many(size_t n, char *s1, const char *s2, ...);

#undef _rmemcpy
extern void *_rmemcpy(void *dest, const void *source, size_t n);
#if __HIGHC__
#define _rmemcpy(dest, source, size) (_move_right(source, dest, size), dest)
#endif

#undef _rstrcpy
extern char *_rstrcpy(char *dest, const char *source);
#if __HIGHC__
#define _rstrcpy(dest, source) \
   (_move_right(source, dest, _find_char(source, _MAXSTRING, 0)+1), dest)
#endif

extern char *_rstrncpy(char *dest, const char *source, size_t n);

extern char *_strncat(char *s1, const char*s2, size_t n);

extern char * strerror(int errnum);
extern char *strstr(const char *s1, const char *s2);
extern size_t strxfrm(char *s1, const char *s2, size_t n);
extern int strcoll(const char *s1, const char *s2);
