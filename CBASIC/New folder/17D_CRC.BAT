IF EXIST C:\CHKCRC.ERR ERASE C:\CHKCRC.ERR
IF EXIST C:\CHKCRC.MIS ERASE C:\CHKCRC.MIS
ECHO CRC CHECKING 17D_IMNT.ZIP >>%1
IF EXIST C:\ADX_UMNT\17D_IMNT.ZIP {
 CRC32 C:\ADX_UMNT\17D_IMNT.ZIP -1144021755 >>%1
 IF EXIST C:\CRC.ERR {
  ECHO 17D_IMNT.ZIP HAS FAILED CRC CHECK >>%1
  ECHO > C:\CHKCRC.ERR
 }
 IF EXIST C:\CRC.OK {
  ECHO 17D_IMNT.ZIP HAS PASSED CRC CHECK >>%1
 }
}
ELSE {
 ECHO C:\ADX_UMNT\17D_IMNT.ZIP WAS NOT FOUND >>%1
 ECHO > C:\CHKCRC.MIS
}
ECHO CRC CHECKING 17D_SMNT.ZIP >>%1
IF EXIST C:\ADX_UMNT\17D_SMNT.ZIP {
 CRC32 C:\ADX_UMNT\17D_SMNT.ZIP 1069801838 >>%1
 IF EXIST C:\CRC.ERR {
  ECHO 17D_SMNT.ZIP HAS FAILED CRC CHECK >>%1
  ECHO > C:\CHKCRC.ERR
 }
 IF EXIST C:\CRC.OK {
  ECHO 17D_SMNT.ZIP HAS PASSED CRC CHECK >>%1
 }
}
ELSE {
 ECHO C:\ADX_UMNT\17D_SMNT.ZIP WAS NOT FOUND >>%1
 ECHO > C:\CHKCRC.MIS
}
ECHO CRC CHECKING 17D_ROOT.ZIP >>%1
IF EXIST C:\ADX_UMNT\17D_ROOT.ZIP {
 CRC32 C:\ADX_UMNT\17D_ROOT.ZIP 431926221 >>%1
 IF EXIST C:\CRC.ERR {
  ECHO 17D_ROOT.ZIP HAS FAILED CRC CHECK >>%1
  ECHO > C:\CHKCRC.ERR
 }
 IF EXIST C:\CRC.OK {
  ECHO 17D_ROOT.ZIP HAS PASSED CRC CHECK >>%1
 }
}
ELSE {
 ECHO C:\ADX_UMNT\17D_ROOT.ZIP WAS NOT FOUND >>%1
 ECHO > C:\CHKCRC.MIS
}
ECHO CRC CHECKING 17D_UMNT.ZIP >>%1
IF EXIST C:\ADX_UMNT\17D_UMNT.ZIP {
 CRC32 C:\ADX_UMNT\17D_UMNT.ZIP -414666197 >>%1
 IF EXIST C:\CRC.ERR {
  ECHO 17D_UMNT.ZIP HAS FAILED CRC CHECK >>%1
  ECHO > C:\CHKCRC.ERR
 }
 IF EXIST C:\CRC.OK {
  ECHO 17D_UMNT.ZIP HAS PASSED CRC CHECK >>%1
 }
}
ELSE {
 ECHO C:\ADX_UMNT\17D_UMNT.ZIP WAS NOT FOUND >>%1
 ECHO > C:\CHKCRC.MIS
}
:ABORT
