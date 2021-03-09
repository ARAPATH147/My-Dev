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
                    if ((typeof(pedNum) == 'undefined') || (pedNum == null) || 
					           (pedNum == "") || (pedLength < 8) || (pedValue == 0))  {	
                          					   
                                count = count + 1 ; 

               }
           }
       }
   }
    

    alert(lines.length + " PED numbers has been checked ," + " Found " + count + " invalid entries");



var xhttp = new ActiveXObject("Microsoft.XMLHTTP");
xhttp.open("POST", "http://centd5241kl5/Gateway/api/applicationstatus", true);
xhttp.setRequestHeader("Content-Type", "application/json"); 
var data = "{ \"Identifier\": \"EPOS_8c6110547738a6d35d5ad05ae94\", \"Status\" : 10, \"StatusMessage\" : \"Your success message\" }";
xhttp.send(data);

alert("completed");

document.getElementById("output").appendChild(table);
}

