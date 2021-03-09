//************************************************************
//************************************************************
//  READPED.CSV      Ranjith Goplankutty       07-10-2016    *
//  this module is being written to prove the POC of         * 
//  calling webservice using  PED data and alert to show     *
//  it on the dash board status.                             *
//************************************************************
//************************************************************

function handleFiles(files) {
	// Check for the various File API support.
	if (window.FileReader) {
		// FileReader are supported.
		getAsText(files[0]);
	} else {
		alert('FileReader are not supported in this browser.');
	}
}

function getAsText(fileToRead) {
	var reader = new FileReader();
	// Handle errors load
	reader.onload = loadHandler;
	reader.onerror = errorHandler;
	// Read file into memory as UTF-8      
	reader.readAsText(fileToRead);
}

function loadHandler(event) {
	var csv = event.target.result;
	processData(csv);             
}

function processData(csv) {
    var allTextLines = csv.split(/\r\n|\n/);
    var lines = [];
    while (allTextLines.length) {
        lines.push(allTextLines.shift().split(','));
    }
    console.log(lines);
	drawOutput(lines);
}

function errorHandler(evt) {
	if(evt.target.error.name == "NotReadableError") {
		alert("Canno't read file !");
	}
}

$.get('PEDAUD.CSV', function(data) {
	var rows = data.split("\n");
	rows.forEach( function getvalues(thisRow) {
	
	 alert("thisRow");
		
	var columns = thisRow.split(",");
//for(var i=0;i<columns.length;i++){ 
	
	     
	})	
 })

function drawOutput(lines){
	//Clear previous data
	document.getElementById("output").innerHTML = "";
	var table = document.createElement("table");
	var count = 0	
	
	for (var i = 0; i < lines.length; i++) {	   
		    
		//alert(lines[i]);   
		var row = table.insertRow(-1);
		for (var j = 0; j < lines[i].length; j++) {		 
			
					var firstNameCell = row.insertCell(-1);
					firstNameCell.appendChild(document.createTextNode(lines[i][j]));			
					
			if(j==5){
				 var pedNum = lines[i][j]
				 var pedLength = pedNum.length
				 var pedValue = parseInt(lines[i][j]);   

				 
				 // checking All rows and making sure the PED Numbers have been parsed right and validated fine
					if ((typeof(pedNum) == 'undefined') || (pedNum == null) || (pedNum == "") || (pedLength < 8) || (pedValue == 0))  {	
					 					 
						count = count + 1 ;                        			
					
				}				
			}	               				 
		}	 		
	}
	document.getElementById("output").appendChild(table);
	
	alert(lines.length + " PED numbers has been checked ," + " Found " + count + " invalid entries");
}


//function callService(){
//	
//	var objXMLHttpRequest = createXMLHttpRequest();
//	objXMLHttpRequest.open("method", "<serviceUrl>", false);
//	objXMLHttpRequest.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
//	var packet = '<?xml version="1.0" encoding="utf-8" ?> <soap:Envelope xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/"> <soap:Body> <PieTable xmlns="[WebService(Namespace = "http://localhost:2900/")]"> <table>Pie</table> </PieTable> </soap:Body> </soap:Envelope>'; 
//	objXMLHttpRequest.send(packet);
//	var result = objXMLHttpRequest.responseXML; 
//}


//function createXMLHttpRequest() {
//   if (typeof XMLHttpRequest != "undefined") {
//        //All modern browsers (IE7+, Firefox, Chrome, Safari, and Opera) uses XMLHttpRequest object
//        return new XMLHttpRequest();
//    }
//    else if (typeof ActiveXObject != "undefined") {
//        //Internet Explorer (IE5 and IE6) uses an ActiveX Object
//        return new ActiveXObject("Microsoft.XMLHTTP");
//    }
//    else {
//       throw new Error("XMLHttpRequestnot supported");
//    }
//}


