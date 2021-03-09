#Root logger option
log4j.rootLogger=info, file

log4j.appender.file=org.apache.log4j.DailyRollingFileAppender
log4j.appender.file.File= D:/ADX_UDT1/DECSERV
log4j.appender.file.DatePattern= '.'E
log4j.appender.file.layout=org.apache.log4j.PatternLayout
log4j.appender.file.layout.ConversionPattern= %d{yyyy-MM-dd HH:mm:ss,SSS} [%t] %p %c - %m%n

#Send KPI LOG format
log4j.logger.SendKPILogger = debug, SendKPILogger
log4j.appender.SendKPILogger = org.apache.log4j.DailyRollingFileAppender
log4j.appender.SendKPILogger.DatePattern = '.'E
log4j.appender.SendKPILogger.layout = org.apache.log4j.PatternLayout
log4j.appender.SendKPILogger.layout.ConversionPattern = %m%n
log4j.appender.SendKPILogger.File =D:/ADX_UDT1/SENDKPI
log4j.additivity.SendKPILogger = false

#Rec KPI LOG format
log4j.logger.RecKPILogger = debug, RecKPILogger
log4j.appender.RecKPILogger = org.apache.log4j.DailyRollingFileAppender
log4j.appender.RecKPILogger.DatePattern = '.'E
log4j.appender.RecKPILogger.layout = org.apache.log4j.PatternLayout
log4j.appender.RecKPILogger.layout.ConversionPattern = %m%n
log4j.appender.RecKPILogger.File = D:/ADX_UDT1/RECDKPI
log4j.additivity.RecKPILogger = false
