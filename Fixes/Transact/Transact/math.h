#define HUGE_VAL 1.67e308
/*      High C version:
#define _PI 3.14159_26535_89793_23846_26433_83279_50288_41971_69399_37510
#define _E  2.71828_18284_59045_23536_02874_71352_66249_77572_47093_69995
*/
#define _PI 3.14159265358979323846264338327950288419716939937510
#define _E  2.71828182845904523536028747135266249775724709369995


extern double cos(double x);
extern double sin(double x);
extern double tan(double x);
extern double atan(double x);

extern double exp(double x);
extern double log(double x);
extern double sqrt(double x);

extern double acos(double x);
extern double asin(double x);

extern double cosh(double x);
extern double sinh(double x);
extern double tanh(double x);

extern double log10(double x);

extern double fabs(double x);
#undef abs
extern int abs(int i);
#if __HIGHC__ 
#define abs(i) (_abs(i))
#endif

extern double ceil(double x);
extern double floor(double x);

extern double atan2(double y, double x);

extern double frexp(double value, int *exp);
extern double ldexp(double x, int exp);
extern double modf(double value, double *iptr);

extern double pow(double x, double y);

extern double fmod(double x, double y);
