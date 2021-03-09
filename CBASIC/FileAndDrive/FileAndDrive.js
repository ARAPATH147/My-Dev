//---------------------------------------------------------------------------------------
//File and Drive status -       Ranjith Gopalankutty                    14-04-2017
// 
// This JavaScript is written as a replacement for the original critical
// file status component written for EPOS keyed file status. As the original input 
// file was being produced only once a month. The purpose of the script was never
// being materialized.  
// Below new script is written to read through the File&Drive report mainframe
// produces on dailys basis.
// Belows are the data it checks
// if keyed file performance or full affected per file stores count is more than 25
// If log exception is more than 5 (amber) or 10 (red)
// if Drive failures affected stores are more than 5(red condition) and 3(amber condition)
//
//  Note :1.Check configuration file name.
//      2.json2.js is exists in the location
//      3.add errorLogFileLocation file path
//----------------------------------------------------------------------------------------
var componentAPI;
var componentIdentifier;
var staticReportIdentifier;
var staticReportAPI;
var fileLocation;
var fileMoveLocation; 
var isFileError = 0;  
var successMessage;
var warningMessage;
var errorMessage;
var fileNotExistsMessage;
var fileEmptyMessage; 
var successValue;
var warningValue;
var errorValue
var referencedJsonFilepath="json2.js";
var errorLogFileLocation="C:/FileAndDrive/Log.txt";
// Creates the File object
var fileObject = new ActiveXObject("Scripting.FileSystemObject");
//referring another file for JSON object Conversion
var libReference = eval(fileObject.OpenTextFile(referencedJsonFilepath, 1).ReadAll());
var whiteSpaceRegEx = new RegExp("\\s+","gm");
var multiWhiteSpaceRegEx = new RegExp("\\ {2,}","gm");//To replace multiple whitespaces.
var removeDoubleQuotesRegEx = new RegExp('\\"+',"gm");
//**********************************************************//

readFileHealthStatus();

//initial method to call
function readFileHealthStatus()
{
    if(readConfiguration())
    {
       readFileContent(fileLocation);
    }
}

//read FileAndDrive.json
function readConfiguration()
{
    if(fileObject.FileExists("FileAndDrive.json"))
    {
        var missingKeys=[];
        var fileSpec = fileObject.GetFile("FileAndDrive.Json");
        var openFile = fileSpec.OpenAsTextStream( 1, 0 );

        try
        {
            var dataObj = openFile.ReadAll();

        }
        catch (e) 
       {
            writeLog(" There is no configuration file")                 //RKG
            return false;
       }

        openFile.Close();

        try
        {
            var jsonObj = JSON.parse(dataObj);

        }
        catch (e) 
       {
            writeLog("invalid Configuration file");
            return false;
       }

        //-----------------------reading keys-------------------------------------------------------------------------------------------------------------------			
        jsonObj.hasOwnProperty("componentAPI")          ?   (componentAPI=jsonObj.componentAPI ): (missingKeys.push("componentAPI"));
        jsonObj.hasOwnProperty("componentIdentifier")   ?   (componentIdentifier=jsonObj.componentIdentifier ): (missingKeys.push("componentIdentifier"));
        jsonObj.hasOwnProperty("staticReportIdentifier")?   (staticReportIdentifier=jsonObj.staticReportIdentifier ): (missingKeys.push("staticReportIdentifier"));
        jsonObj.hasOwnProperty("staticReportAPI")       ?   (staticReportAPI=jsonObj.staticReportAPI ): (missingKeys.push("staticReportAPI"));
        jsonObj.hasOwnProperty("fileLocation")          ?   (fileLocation=jsonObj.fileLocation ): (missingKeys.push("fileLocation"));
        jsonObj.hasOwnProperty("fileMoveLocation")      ?   (fileMoveLocation=jsonObj.fileMoveLocation ): (missingKeys.push("fileMoveLocation"));         
        jsonObj.hasOwnProperty("successMessage")        ?   (successMessage=jsonObj.successMessage ): (missingKeys.push("successMessage"));
        jsonObj.hasOwnProperty("successValue")          ?   (successValue=jsonObj.successValue ): (missingKeys.push("successValue"));
        jsonObj.hasOwnProperty("warningMessage")        ?   (warningMessage=jsonObj.warningMessage ): (missingKeys.push("warningMessage"));
        jsonObj.hasOwnProperty("warningValue")          ?   (warningValue=jsonObj.warningValue ): (missingKeys.push("warningValue"));
        jsonObj.hasOwnProperty("errorMessage")          ?   (errorMessage=jsonObj.errorMessage ): (missingKeys.push("errorMessage"));
        jsonObj.hasOwnProperty("errorValue")            ?   (errorValue=jsonObj.errorValue ): (missingKeys.push("errorValue"));
        jsonObj.hasOwnProperty("fileNotExistsMessage")  ?   (fileNotExistsMessage=jsonObj.fileNotExistsMessage ): (missingKeys.push("fileNotExistsMessage"));        
        jsonObj.hasOwnProperty("fileEmptyMessage")      ?   (fileEmptyMessage=jsonObj.fileEmptyMessage ): (missingKeys.push("fileEmptyMessage"));         
        //-----------------------end of reading keys------------------------------------------------------------------------------------------------------------
        
        //checks missing keys
            if(missingKeys.length > 0)
            {
                var key ="conguration file has missing key , ";
                for(i=0;i<missingKeys.length;i++)
                {
                   key=key + " \n " + (i+1) + "." + missingKeys[i];
                }
                   writeLog(key);
                   
                   return false;
             }
        }
        else
        {
            writeLog("FileAndDrive.json file doesn't exist ! ");
            return false;
        } 
        return true;
}

  function readFileContent(filepathVal)
{ 
      var filePath=filepathVal;   
      var fileObj = new ActiveXObject("Scripting.FileSystemObject");    
      if(fileObj.FileExists(filePath))
      {
       
            var fileSpec = fileObj.GetFile(filePath);             
            var openFile = fileSpec.OpenAsTextStream( 1, 0 );
            var isFileEmpty=true;           
            var count = 0;       
            var J  = 0;
            var I  = 1;
            var L = 0;
            var M = 0;
            var FileName = [];
            var CountperFile = [];
            var CountOfStore = [];
            var readOperation = 0;
            var FileCheck = "Dummy";
            var ErrorFile = 0;
            var StoreNumArr = [];

while( !openFile.AtEndOfStream )

{

             var Count = 0;
             var StoreNumber = [];
            // Below loop is designed to read from where the data begins. As per mainframe report the data should start immediately after one blank line
            // if that breakes, complete read logic will fail. 
                if (readOperation == 0)
                  {
                     
                     var readLine = openFile.ReadLine();
                     readLine = openFile.ReadLine();
                     readOperation = readOperation + 2; 

                   }

                     var ReportTitle = readLine.substring(0,22);

                     if ((ReportTitle.match("KEYED FILE IS FULL"))) 
                      {

                         FileName[I] = readLine.substring(21,readLine.length)
                         readLine = openFile.ReadLine();
                         readLine = openFile.ReadLine();

                         I = I+ 1
                         J = J + 1

                       }
                     else if ((ReportTitle.match("KEYED FILE PERFORMANCE")))
            
                       {
            
                       FileName[I] = readLine.substring(25,readLine.length)
                       readLine = openFile.ReadLine();
                       readLine = openFile.ReadLine(); 
                       I = I+ 1 
                       J = J + 1 

                       }

                      else if ((ReportTitle.match("DRIVE")))

                       { 

                        var DriveName = [];
                        var Drive;
                        var DriveCount = 0;
                        var DriveStore = []                      
                        
                      
                        while (!ReportTitle.match("KEYED")  && !ReportTitle.match("LOG"))
                              
                           { 
                              Drive = readLine.substring(0,5) 
                               
                              if ( Drive == "DRIVE")
                              
                               {
                                   L = L + 1
                                   DriveName[L] = readLine.substring(16,17)                              
                               
                                
                                } 
                              
                                readLine = openFile.readLine();
                                Drive = readLine.substring(0,5) 
                              
                              if(!isNaN(readLine.substring(0,5)) && (readLine.substring(0,5) != null) && (readLine.substring(0,5) != 'undefined') && (readLine.substring(0,5) != ""))							   
                               {  
                              
                               
                                  if (DriveStore[L] == 'undefined' || DriveStore[L] == null || DriveStore[L] == "") 
                                    {
                                       DriveStore[L]  = readLine.substring(0,5)  
                                       DriveCount = DriveCount + 1  
                              
                                    }
                                    
                                    else
                                    { 
                                
                                        DriveStore[L] = DriveStore[L] + " " + readLine.substring(0,5)
                                        DriveCount = DriveCount + 1
                                 
                                    }
                                    
                                    }
                                ReportTitle = readLine.substring(0,22)  
                                  
                                
                           }
                        
                       } 
                        

                            var LineString = readLine.substring(0,6);

                            while ((!LineString.match("KEYED")) && (!openFile.AtEndOfStream)) 

                            {   

                             StoreNumber = readLine.substring(0,5)
                            
                             if ( (LineString != null) && (LineString != 'undefined') &&  (LineString != "") && (!isNaN(StoreNumber)) )
                            
                            
                                 {
                             
                                  if ((StoreNumArr[J] == 'undefined') || (StoreNumArr[J] == null) ||(StoreNumArr[J] == ""))
                                     {
                                                                                    
                                          StoreNumArr[J] = StoreNumber;
                                          Count = Count + 1
                                     }
                                  else
                            
                                    {
                                     
                                            StoreNumArr[J] = StoreNumArr[J] + " " +  StoreNumber ;
                                            Count = Count + 1
                            
                                    }
                            
                                    }
                            
                                   CountOfStore[I-1] = Count;
                                   
                                   readLine = openFile.ReadLine();
                            
                                   
                            
                                   LineString = readLine.substring(0,6)
                                  
                                   if (LineString.match("LOG"))
                                                        
                                       {
                                           var LogName = [];
                                           var CountLog = 0
                                           M = M + 1

                                                                          
                                           while(!openFile.AtEndOfStream)
                                             
                                           { 
                                                                      
                                            if(!isNaN(readLine.substring(0,5)) && (readLine.substring(0,5) != null) && (readLine.substring(0,5) != 'undefined') && (readLine.length > 0))					                  
                                              
                                            {     
                                              if (LogName[M] == null) 
                                     
                                               {                                     

                                                        LogName[M] = readLine.substring(0,5)
                                                        CountLog = CountLog + 1
                                               }

                                              else
                                               {
                                                       LogName[M] = LogName[M] + " " + readLine.substring(0,5)
                                                       CountLog = CountLog + 1 

                                               }

                                             }
                                               
                                                      readLine = openFile.ReadLine();
                                             } 
  
                                              //for last line addition to the array adding the line from here 
 
                                              if(!isNaN(readLine.substring(0,5)) && (readLine.substring(0,5) != null) && (readLine.substring(0,5) != 'undefined') && (readLine.length > 0))

                                                 {
                                                     LogName[M] = LogName[M] + " " + readLine.substring(0,5)
                                                     CountLog = CountLog + 1
                    
                                                 }

                                       }
                                             
                            } 
 
}     


// Below loop reads through complete file and sets the counter
// Rules are as below
// If an affected file count is more than 25 stores then the report
// will set a counter against ErrorFile and if the cumulative count
// is more than 12 store then it qualifies for amber condition
// if the affected store count is more than 25 then it is red condition

 var K =1;
        
           
 if(I > 1)

   {
          for (K=1 ; K <= I ; K++)
           {  

                   if (CountOfStore[K] >= 25)

           {
                     ErrorFile = ErrorFile + 1
                     //writeLog((FileName[K]) + " " + (StoreNumArr[K]) + " " + (CountOfStore[K])) 
           }
           }
    }
 
if (L == 0 && I == 1 && M ==0) 

     {
            writeLog("No data in the source File, please check if file is generated")
            isFileError = 1
 
     }
 
//  Below loop is for checking the errors for Drive failures
//  if affected store count is more than 3 then it is amber condition
//  if the affected store count is more than 5 then it alone qualifies
//  for red condition 
   
 
 if (L > 0)

  {
      K = 1;

      for (K =1 ; K <= L ; K++)
      {
          //writeLog(DriveName[K] + " " + DriveStore[K]  + " " + DriveCount)
      }
  }

 
//  Below loop is to traverse through only Log exceptions . iteration is  
//  simple , if the countlog is more than 10 then it is red condition
//  if count log is between 5 to 10 then it alone qualifies for amber condition
//  if the count is less than 5 then it is green
 
  
 if (M > 0)

  {                 
          //writeLog(LogName[M] + " " + CountLog ) 
  }   

     }
  else 

     {
            writeLog(" There is no input file , please check if source file is generated")             
            isFileError = 2;

     } 
      buildAndSendData(ErrorFile,DriveCount,CountLog,isFileError,"")
}
    
//fileContent : Arrays of unexpected/invalid file entry.
//isFileError : File contains any error or not.
//erMessage : If any error occurs in file, then attach error message also. 
function buildAndSendData(ErrorFile,DriveCount,CountLog,isFileError,erMessage)
{
   var statusObject = {};
   var ErrorData=[];
   statusObject.Identifier = componentIdentifier;       
   
       if(isFileError == 1)
        {
            statusObject.Status = 15;
            statusObject.StatusMessage = fileEmptyMessage.replace("{date}",Date());
            ErrorData.push([""+statusObject.StatusMessage+""]);  
             
        }
       else if(isFileError == 2)
        {
            //statusObject.Status = 15;
            statusObject.StatusMessage = fileNotExistsMessage.replace("{date}",Date());
            ErrorData.push([""+statusObject.StatusMessage+""]);  
             
        }
       else if (( ErrorFile > 25) && (DriveCount > 5) && (CountLog > 10)) 
 
        {    
            statusObject.Status = errorValue;            
            statusObject.StatusMessage = errorMessage.replace("{ErrorFile}",ErrorFile).replace("{DriveCount}",DriveCount).replace("{CountLog}",CountLog).replace("{date}",Date());
            ErrorData.push([""+statusObject.StatusMessage+""]);

        }

        else if (( ErrorFile > 12) && (DriveCount > 3) && (CountLog > 5)) 
        {            

            statusObject.Status = warningValue;     
            statusObject.StatusMessage = warningMessage.replace("{ErrorFile}",ErrorFile).replace("{DriveCount}",DriveCount).replace("{CountLog}",CountLog).replace("{date}",Date());;
            ErrorData.push([""+statusObject.StatusMessage+""]);
        }
        else 

        {  
            statusObject.Status = successValue;             
            var mesgData=  successMessage.replace("{date}",Date());
            ErrorData.push([""+mesgData+""]); 

        }

    ajaxFunction(componentAPI,statusObject);

    var errorData =  {};
    errorData.Identifier = staticReportIdentifier;
    errorData.Data= JSON.stringify(ErrorData);	

    ajaxFunction(staticReportAPI,errorData);
  
    moveFile(fileLocation,fileMoveLocation);
}


function ajaxFunction(urlPost,data){

    var ajaxRequest;  // The variable that makes Ajax possible! 

    try
    {
       // Opera 8.0+, Firefox, Safari
      ajaxRequest = new XMLHttpRequest();
    }
   catch (e)
    {      
      // Internet Explorer Browsers
     try
      {
       ajaxRequest = new ActiveXObject("Msxml2.XMLHTTP");
      }
      catch (e) 
     {         
         try
         {
           ajaxRequest = new ActiveXObject("Microsoft.XMLHTTP");
         }
         catch (e)
         {         
            // Something went wrong
            writeLog("error in creating ActiveXObject");
            return false;
         }
     }
    }
 
 ajaxRequest.onreadystatechange = function(){
  
      if(ajaxRequest.readyState == 4)
      {   
        //WSH.Echo(ajaxRequest.responseText);
      }
   }

    try
    {
        ajaxRequest.open('POST', urlPost, false);//false is for synchronous 
        ajaxRequest.setRequestHeader('Content-Type', 'application/json');
        ajaxRequest.send(JSON.stringify(data));
    }
    catch(e)
    {
        writeLog(urlPost + " API is not available or down at this moment");
    }
     
}


//Move file
function moveFile(moveFrom,moveTo)
{        
        try
        {
            var fiObject = new ActiveXObject("Scripting.FileSystemObject");

            if(fiObject.FileExists(moveFrom))
            {
                var file = fiObject.GetFile(moveFrom);
                if(fiObject.FolderExists(moveTo))
                {
                    var currentDate = new Date();
                    var buildFileName = (file.name).split('.')[0];
                    buildFileName = buildFileName.concat(
                    "_",currentDate.getDate(),
                    "_",((currentDate.getMonth())+1),
                    "_",currentDate.getFullYear(),
                    "_",currentDate.getHours(),
                    "_",currentDate.getMinutes(),
                    "_",currentDate.getMilliseconds(),
                    '.',fiObject.GetExtensionName(moveFrom));

                }
                else
                {
                   writeLog("Doesn't exists the destination folder.");
                }

           }
            else
              {
               writeLog("File doesn't exist to move.");	
            }
        }
        catch(e)
        {
            writeLog("file couldn't to move.");
        }               
}

function writeLog(logMessage)
{
    try
     {
        var fileObj = new ActiveXObject("Scripting.FileSystemObject");
        var logFile;

        if(fileObj.FileExists(errorLogFileLocation))
        {
            var fileSpec = fileObj.GetFile(errorLogFileLocation);
            logFile = fileSpec.OpenAsTextStream( 8, 0 );
        }
         else
        {
            logFile= fileObj.CreateTextFile(errorLogFileLocation, true);
        }
        
        logFile.WriteLine(Date());
        logFile.WriteLine("----------------------------------------------------------------------------");
        logFile.WriteLine(logMessage);
        logFile.WriteLine(' ');
        logFile.WriteLine(' ');
        logFile.Close();
    }
    catch(e)
    {
         return true;
    }
}

