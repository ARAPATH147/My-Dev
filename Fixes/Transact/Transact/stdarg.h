#define va_list char *

/* Minimum size of a parameter:  round to the number of ints; must be at least one int. */
#if !__HIGHC__
#define _max(a,b) ((a)>(b)?(a):(b))
#endif
#define __psize(parmsize) _max(sizeof(int), ((parmsize)+sizeof(int)-1)/sizeof(int)*sizeof(int) )

#define va_start(ap, parmN) ( ap = (char *) &parmN + __psize(sizeof(parmN)) )

#define va_arg(ap, type) ( \
	   *(type *) ((ap += __psize(sizeof(type)) ) - __psize(sizeof(type)))  )

#define va_end(ap)
