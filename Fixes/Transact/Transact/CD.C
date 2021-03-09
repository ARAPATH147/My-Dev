#include "transact.h" //SDH 19-May-2006

// Generate a boots check digit
// bc   = 3 byte right aligned BCD boots code (i.e. cc cc cc)
// bccd = 4 byte right aligned BCD boots code with check dig (i.e. 0c cc cc cd)
void calc_boots_cd(BYTE *bccd, BYTE *bc)
{
   LONG tot, i, nb;

   memset(bccd, 0x00, 4);
   tot=0L;
   for (i=0; i<3; i++) {
      // high nibble
      nb=(*(bc+i)&0xF0)>>4;
      *(bccd+i)|=nb;
      tot+=((7-(i*2))*nb);
      // low nibble
      nb=*(bc+i)&0x0F;
      *(bccd+i+1)|=(nb<<4);
      tot+=((6-(i*2))*nb);
   }
   tot=11-(tot%11);
   if (tot<10) {
      nb=tot;
   } else {
      nb=0;
   }
   *(bccd+3)|=nb;

}

// Generate an EAN13 check digit
// bc   = 6 byte right aligned BCD bar code (i.e. cc cc cc cc cc cc)
// bccd = 7 byte right aligned BCD bar code with check dig
//        (i.e. 0c cc cc cc cc cc cd)
void calc_ean13_cd(BYTE *bccd, BYTE *bc)
{
   LONG tot, i, nb;

   //if (debug) {
   //   disp_msg(" barcode :");
   //   dump((BYTE *)bc, 6);
   //}

   memset(bccd, 0x00, 7);
   tot=0L;
   for (i=0; i<6; i++) {
      // high nibble
      nb=(*(bc+i)&0xF0)>>4;
      *(bccd+i)|=nb;
      tot+=nb;
      // low nibble
      nb=*(bc+i)&0x0F;
      *(bccd+i+1)|=(nb<<4);
      tot+=(nb*3);
   }
   tot=10-(tot%10);
   if (tot<10) {
      nb=tot;
   } else {
      nb=0;
   }
   *(bccd+6)|=nb;

   //if (debug) {
   //   disp_msg(" barcode(with cd) :");
   //   dump((BYTE *)bccd, 7);
   //}

}
