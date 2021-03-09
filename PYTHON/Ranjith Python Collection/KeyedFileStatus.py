from struct import *
import sys
import os

def KeyedFileReadStat(firstsector):
        if firstsector:
                blocks = (unpack('<L',firstsector[42:46]))[0]
                #print(str(blocks))
                reclength = (unpack('<H',firstsector[46:48]))[0]                                
                #print(str(reclength))
                MaxRecs = (blocks - 1) * int(508/reclength)
                #print (str(MaxRecs))
                totaladd = firstsector[76:80]
                totaldel = firstsector[88:92]
                TotalRecs = (unpack('<L',totaladd))[0] - (unpack('<L',totaldel))[0]
                #print(TotalRecs)
                return(str(MaxRecs) + ',' + str(TotalRecs))
        else:
                return('Invalid input')

def KeyedFileRecordCalc(Location):
        filecount = 0
        path1 = Location
                
        print ('checking the files in ' + path1)
        output = open(path1 + '/FileStatus.csv', 'w')
        output.write('Store' + ',' + 'Threshold' + ',' + 'Records Present' + '\n')
        for filename in os.listdir(path1):
                if filename[0:4] == 'STOR':
                        filecount += 1
                        try:
                                with open(path1 + filename, 'rb') as f:
                                        chunk = f.read(512)
                                        if chunk:
                                                output.write(filename[5:9] + ',' + KeyedFileReadStat(chunk) + '\n')
                                                print('files processed ', filecount)
                        except Exception as e:
                                print(str(e))
               
        output.close()
        print('Total files processed ', filecount)
        print('Created output file ', path1 + '/FileStatus.csv')

#If the script is directly called
if __name__ == "__main__" :
        if len(sys.argv) > 1:
                Locaiton = str(sys.argv[1]) + '/'
        else:
                print('Input the location of all the keyed files')
                print('- filename should start with STORE1234')
                Location = input('Location:')  + '/'
        KeyedFileRecordCalc(Location)
