/************************************************************************
 *   HC286.PRO                      *
 *                         *
 *      Any dependencies on 'HC' should be referred to as __HIGHC__  *
 *      Use #if defined (__HIGHC__)  and not TRUE / FALSE statements    * 
 *                         *
 ************************************************************************/
#if defined ( __HIGHC__ )

/* Select the CPU to be used */

#define  IAPX286  1     /* Intel 286 Protected Mode   */

/************************************************************************
 *                         *
 * Set up Pragmas specific to FlexOS.  These allow the compiler to      *
 * generate optimized instructions which are specific to the indicated  *
 * CPU.                          *
 *                         *
 ************************************************************************/

pragma off (286);
pragma on (386);
pragma memory_model(Large);
pragma on(segmented_pointer_operations) ;
pragma off(Floating_point);
pragma off(emit_names);
pragma off(check_stack);
pragma off(optimize_for_space);
pragma off(public_var_warnings);
pragma Code("CODE");
pragma On(Quiet);

pragma Off(Align_members);        /* Align structures */
pragma Off(Data_seg_fixup_warn);  /* Warn if there is data segment fixups */
pragma On(Make_externs_global);
pragma Off(optimize_fp);
pragma Off(optimize_xjmp);
pragma Off(optimize_xjmp_space);
pragma On(Codeview);
pragma On(pcc_msgs);

/* Support for mixed model programming */

   /* Near Routine */
#define  RNEAR _CC(_DEFAULT_CALLING_CONVENTION | _NEAR_CALL )
   /* Far Routine  */
#define  RFAR  _CC(_DEFAULT_CALLING_CONVENTION | _FAR_CALL )
   /* Near Data */
#define DNEAR  _near
   /* Far Data */
#define DFAR   _far

#endif

/*  EOF: HC286.PRO  */
