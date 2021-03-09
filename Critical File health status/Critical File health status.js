//---------------------------------------------------------------------------------------
//Critical File health status

//Note :1.Check configuration file name.
//		2.json2.js is exists in the location
//		3.add errorLogFileLocation file path
//----------------------------------------------------------------------------------------
var componentAPI;
var componentIdentifier;
var staticReportIdentifier;
var staticReportAPI;
var fileLocation;
var fileMoveLocation;
var recordLooksFineMessage;
var chainErrorDescription;
var recordErrorDescription;
var longestChainDescription;
var successMessage;
var warningMessage;
var errorMessage;
var fileNotExistsMessage;
var fileEmptyMessage;
var fileIsCorruptedMessage;
var successValue;
var warningValue;
var errorValue;
var storeNumberIndex;
var fileNameIndex;
var totalRecordCapacityIndex;
var usedRecordsIndex;
var longestChainIndex;
var totalStoreCount=0;
var columnNameLabel1;

var longestChainCountSameStore=0;
//Using to check if more than 12 files in a store has the same status (file has longest chain equal to or greater than ‘6’) at end of a set of store
var hasLongestChainCountSameStore =false;
//using to check If any of the file has longest chain equal to or greater than ‘6’
var hasLongestChainCount =false;
var referencedJsonFilepath="json2.js";
var errorLogFileLocation="C:/temp/Log.txt";
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
		// WSH.ECHO("Configuration read successfully")                           // RKG
	{
		readFileContent(fileLocation);
	}
}

//read CriticalFileHealthStatus_config.json
function readConfiguration()
{
	if(fileObject.FileExists("CriticalFileHealthStatus_config.json"))
	{
		var missingKeys=[];		
		var fileSpec = fileObject.GetFile("CriticalFileHealthStatus_config.json");	
		var openFile = fileSpec.OpenAsTextStream( 1, 0 );
		
		try
		{
			var dataObj = openFile.ReadAll();
			
			// WSH.ECHO(dataObj)                                           //RKG
		}
		catch (e) 
		{
			// writeLog("CriticalFileHealthStatus_config.json is empty");
			WSH.ECHO(" There is no configuration file")                 //RKG
			return false;
		}
		
		openFile.Close();
		
		try
		{
			var jsonObj = JSON.parse(dataObj);		 
			
		                                          
		}
		catch (e) 
		{			
			writeLog("invalid CriticalFileHealthStatus_config.json file");
			WSH.ECHO("failed")                                         //RKG  
			return false;
		}
		
		//-----------------------reading keys-------------------------------------------------------------------------------------------------------------------			
		jsonObj.hasOwnProperty("componentAPI")			?  	(componentAPI=jsonObj.componentAPI ): (missingKeys.push("componentAPI"));
		jsonObj.hasOwnProperty("componentIdentifier")	?  	(componentIdentifier=jsonObj.componentIdentifier ): (missingKeys.push("componentIdentifier"));
		jsonObj.hasOwnProperty("staticReportIdentifier")?  	(staticReportIdentifier=jsonObj.staticReportIdentifier ): (missingKeys.push("staticReportIdentifier"));
		jsonObj.hasOwnProperty("staticReportAPI") 		?  	(staticReportAPI=jsonObj.staticReportAPI ): (missingKeys.push("staticReportAPI"));
		jsonObj.hasOwnProperty("fileLocation") 			?  	(fileLocation=jsonObj.fileLocation ): (missingKeys.push("fileLocation"));
		jsonObj.hasOwnProperty("fileMoveLocation") 		?  	(fileMoveLocation=jsonObj.fileMoveLocation ): (missingKeys.push("fileMoveLocation"));
		jsonObj.hasOwnProperty("recordLooksFineMessage")?  	(recordLooksFineMessage=jsonObj.recordLooksFineMessage ): (missingKeys.push("recordLooksFineMessage"));
		jsonObj.hasOwnProperty("chainErrorDescription")	?  	(chainErrorDescription=jsonObj.chainErrorDescription ): (missingKeys.push("chainErrorDescription"));
		jsonObj.hasOwnProperty("recordErrorDescription")?  	(recordErrorDescription=jsonObj.recordErrorDescription ): (missingKeys.push("recordErrorDescription"));
		jsonObj.hasOwnProperty("longestChainDescription")?  (longestChainDescription=jsonObj.longestChainDescription ): (missingKeys.push("longestChainDescription"));
		jsonObj.hasOwnProperty("storeNumberIndex") 		?  	(storeNumberIndex=jsonObj.storeNumberIndex ): (missingKeys.push("storeNumberIndex"));
		jsonObj.hasOwnProperty("fileNameIndex") 		?  	(fileNameIndex=jsonObj.fileNameIndex ): (missingKeys.push("fileNameIndex"));
		jsonObj.hasOwnProperty("totalRecordCapacityIndex")?  	(totalRecordCapacityIndex=jsonObj.totalRecordCapacityIndex ): (missingKeys.push("totalRecordCapacityIndex"));
		jsonObj.hasOwnProperty("usedRecordsIndex") 		?  	(usedRecordsIndex=jsonObj.usedRecordsIndex ): (missingKeys.push("usedRecordsIndex"));
		jsonObj.hasOwnProperty("longestChainIndex") 	?  	(longestChainIndex=jsonObj.longestChainIndex ): (missingKeys.push("longestChainIndex"));
		jsonObj.hasOwnProperty("successMessage") 		? 	(successMessage=jsonObj.successMessage ): (missingKeys.push("successMessage"));
		jsonObj.hasOwnProperty("successValue") 			? 	(successValue=jsonObj.successValue ): (missingKeys.push("successValue"));		
		jsonObj.hasOwnProperty("warningMessage") 		? 	(warningMessage=jsonObj.warningMessage ): (missingKeys.push("warningMessage"));		
		jsonObj.hasOwnProperty("warningValue") 			? 	(warningValue=jsonObj.warningValue ): (missingKeys.push("warningValue"));		
		jsonObj.hasOwnProperty("errorMessage") 			? 	(errorMessage=jsonObj.errorMessage ): (missingKeys.push("errorMessage"));
		jsonObj.hasOwnProperty("errorValue") 			? 	(errorValue=jsonObj.errorValue ): (missingKeys.push("errorValue"));		
		jsonObj.hasOwnProperty("fileNotExistsMessage") 	? 	(fileNotExistsMessage=jsonObj.fileNotExistsMessage ): (missingKeys.push("fileNotExistsMessage"));
		jsonObj.hasOwnProperty("fileIsCorruptedMessage")? 	(fileIsCorruptedMessage=jsonObj.fileIsCorruptedMessage ): (missingKeys.push("fileIsCorruptedMessage"));
		jsonObj.hasOwnProperty("fileEmptyMessage") 		? 	(fileEmptyMessage=jsonObj.fileEmptyMessage ): (missingKeys.push("fileEmptyMessage"));		
		jsonObj.hasOwnProperty("columnNameLabel1") 		? 	(columnNameLabel1=jsonObj.columnNameLabel1 ): (missingKeys.push("columnNameLabel1"));
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
		writeLog("CriticalFileHealthStatus_config.json file doesn't exist ! ");		
		return false;
		
		WSH.ECHO("failing")                                               // RKG
	}	
	return true;
}



function readFileContent(filepathVal)
{	
	var filePath=filepathVal;
	//WSH.ECHO(filepathVal)                                                // RKG
	// Creates the File object
	var fileObj = new ActiveXObject("Scripting.FileSystemObject");
	//Returns a File object corresponding to the file in a specified path.
	if(fileObj.FileExists(filePath))
	{
		var fileSpec = fileObj.GetFile(filePath);
		//populates only when the corresponding line is not corrupted one but the PED is invalid
		var invalidContent = new Array();	
		var longestChainArray = new Array();
		//populates only when the corresponding line is corrupted one.
		var badEntries = new Array();
		var errorArrays = new Array();
		var errorArrayRec = new Array();
		var errorArrayLongestChain = new Array();
		var errorArrayLongestChainFiltered = new Array();
		var errorArrayLongestChainStore = new Array();		
		//Opens a specified file and returns a TextStream object that can be used to read from, write to, or append to the file.
		//arg 1 // 1 = Open a file for reading only. You can't write to this file,
		//arg 0 // 0 = Opens the file as ASCII.
		var openFile = fileSpec.OpenAsTextStream( 1, 0 );	
		var isFileEmpty=true;	
		var isContainsBadData = false;
		var isContainsInvalidContent=false;		
		//reading files till the end of files 
		var count = 0;	
		var errorArraysIndex = 0;
		var errorArrayRecIndex = 0;
		var errorArrayLongestChainIndex = 0;
		var errorArrayLongestChainStoreIndex = 0;
		var longestChainArrayIndex = 0;
		var chainArrayIndex = 0;
		var countBadData = 0;		
		var linePickFrom = 0;//to avoid first 3 lines from parsing.
		var storeNo;		
		var J  = 0;
		var I  = 1;
		var FileName = [];
		var CountperFile = [];
		var CountOfStore = [];	 
		var readOperation = 0;		
	    var FileCheck = "Dummy";
		var ErrorFile = 0;
		
		var StoreNumArr = [];

while( !openFile.AtEndOfStream )

{
            	
			WSH.ECHO("TEST")
		    var Count = 0;
	        
					   
		     var StoreNumber = [];     		
		     
		     
			   
			   // linePickFrom=linePickFrom+1;			

               if (readOperation == 0)
			   
			   {

 
		          var readLine = openFile.ReadLine();
			 
			      readOperation = readOperation + 1;		  
				  
				  
				 
				  
			   }
			   
			   
		        var ReportTitle = readLine.substring(0,22);	
				
				 
                 
				
		         if ((ReportTitle.match("KEYED FILE IS FULL"))) 
				 
				 {
					 
					  FileName[I] = readLine.substring(21,readLine.length)
					
					  WSH.ECHO(FileName[I])
					 
				 }
                 else if ((ReportTitle.match("KEYED FILE PERFORMANCE")))

				 {
					   
                       FileName[I] = readLine.substring(25,readLine.length)
					   
					   WSH.ECHO(FileName[I])
				    
					
				 }  

                    I = I+ 1
					
			        J = J + 1
					
					readLine = openFile.ReadLine();					 
					readLine = openFile.ReadLine();
					
					 
		 
					
					 
					var LineString = readLine.substring(0,6);
					
					
              
                     
						
					//while (!LineString.match("KEYED") || (!LineString.match("LOG"))  && (!openFile.AtEndOfStream))  
						
					  while ((LineString != "KEYED") && (LineString.substring(0,4) != "LOG") && (!openFile.AtEndOfStream))
					
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
                       
						 
						 
						       
						       readLine = openFile.ReadLine();	

                                							   
							   LineString = readLine.substring(0,6)	
							   
                        }		   
						 
				    }     
					
                    
                         
                        CountOfStore[I-1] = Count;

                       						
		       
				 }         
 
	 
        
		   
 
		 
	    
	      	   
		var K =1;
        
         	  
		if(I > 1)
			
		{			
		   for (K=1 ; K < I ; K++)
           {			   	    
		     
                if (CountOfStore[K] >= 1)			
			
			    {

                    ErrorFile = ErrorFile + 1			
			         //writeLog((FileName[K]) + " " + (StoreNumArr[K]) + " " + (CountOfStore[K]))				  
		        }
			}
		}
		else
			
		{
	       //writeLog("No data in the source File, please check if file is generated")
	
 						 
        }
		
        
				
	 	
	 
	 
	 
	
 
//fileContent : Arrays of unexpected/invalid file entry.
//isFileError : File contains any error or not.
//erMessage : If any error occurs in file, then attach error message also.
//statusCase : 1 for Red. 2 for Amber. 0 for Green.
function buildAndSendData(fileContent,isFileError,erMessage,statusCase)
{	
	var statusObject = {};	
	var ErrorData=[];	
	statusObject.Identifier = componentIdentifier;	
	
		//case 1 : File is empty.
		//case 2 : File doesn't exist.
		//case 3 : File is corrupted.
		if(isFileError)
		{
			statusObject.Status = 15;
			statusObject.StatusMessage = erMessage;
			ErrorData.push([""+erMessage+""]);
			
			WSH.ECHO("NO FILE");
		}
		else
		{					
			switch(statusCase)
			{
				//------------------------- RED Condition -------------------------------------------------------------------------------
				case 1:
					statusObject.Status = errorValue;
					WSH.ECHO(errorValue);
					errorMessage = errorMessage.replace("{0}",totalStoreCount);
					statusObject.StatusMessage = errorMessage;			
					ErrorData.push([""+columnNameLabel1+""]);						
					var tempStoreNo="";
					var tempFile="";
					var store="";
					var firstCountInRec=0;
					//sending all stores data
					for(i=0;i<fileContent.length;i++)
					{		
						var arrContent=fileContent[i].split(",");						
						//Record issue
						if(arrContent[2]=="Rec")
						{					
							if(tempFile==arrContent[0])
							{							
								store = store + " , " + arrContent[1];								
							}
							else
							{							
								if(firstCountInRec==0)
								{
									store=arrContent[1];
									firstCountInRec=1;																
								}
								else
								{
									var tempStoreData = arrContent[1];
									//previuos item is adding to array
									arrContent[1]=((recordErrorDescription.replace("{0}",tempFile)).replace("{1}",store));
									ErrorData.push([""+arrContent[1] +""]);
									store=tempStoreData;									
								}									
								tempFile = arrContent[0];
							}
							//end of array
							//Current item is adding to array
							if(i==(fileContent.length-1))
							{
								arrContent[1]=((recordErrorDescription.replace("{0}",arrContent[0])).replace("{1}",store));
								ErrorData.push([""+arrContent[1] +""]);										
							}							
						}
						//chain issue
						else
						{						
							tempStoreNo=tempStoreNo + "," +	arrContent[0];
						}
					}

					if(tempStoreNo!="")
					{
						tempStoreNo = tempStoreNo.replace(/(^[,\s]+)|([,\s]+$)/g, '');
						var messageView =  chainErrorDescription.replace("{0}",tempStoreNo);
						ErrorData.push([""+messageView+""]);
					}	
					
					break;
				//------------------------- end of RED Condition -------------------------------------------------------------------------------
				
				
				
				//------------------------- AMBER Condition ----------------------------------------------------------------------------------
				case 2:
					statusObject.Status = warningValue;
					warningMessage = warningMessage.replace("{0}",totalStoreCount);
					statusObject.StatusMessage = warningMessage;					
					ErrorData.push([""+columnNameLabel1+""]);						
					var tempStoreNo="";
					var tempFile="";
					var store="";
					var firstCountInRec=0;									
					//sending all stores data
					for(i=0;i<fileContent.length;i++)
					{		
						var arrContent=fileContent[i].split(",");
						
						if(tempFile==arrContent[0])
						{
								store = store + " , " + arrContent[1];								
						}
						else
						{							
							if(firstCountInRec==0)
							{
								store=arrContent[1];
								firstCountInRec=1;																
							}
							else
							{
								var tempData = arrContent[1];
								arrContent[1]=(longestChainDescription.replace("{0}",tempFile)).replace("{1}",store);
								ErrorData.push([""+arrContent[1] +""]);
								store='';
								store=tempData;									
							}
							
							tempFile = arrContent[0];
						}

						//end of array
						if(i==(fileContent.length-1))
						{
							arrContent[1]=(longestChainDescription.replace("{0}",arrContent[0])).replace("{1}",store);
							ErrorData.push([""+arrContent[1] +""]);
						}					
					}

					break;
				//------------------------- end of AMBER Condition ----------------------------------------------------------------------------------
				
				
				
				//------------------------- GREEN Condition------------------------------------------------------------------------------------------
				case 0:
					statusObject.Status = successValue;
					statusObject.StatusMessage = (successMessage.replace('{0}',totalStoreCount));
					var mesgData= recordLooksFineMessage.replace('{0}',totalStoreCount);
					ErrorData.push([""+mesgData+""]);					
					break;
				//------------------------- end of  GREEN Condition------------------------------------------------------------------------------------------	
			}			
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

					//file.name=buildFileName;
				
					//file.Move(moveTo);					
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

}