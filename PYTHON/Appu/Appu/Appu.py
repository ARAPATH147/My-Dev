# APPU    Pranav Lal                 23-Dec-2018
# This application is written to check if any weights
# between the range of 2kg to 109KG can be measured
# using a scale of A0,A1,A2 .... A100 where A can be
# any integer between 2 to 109, 0 to 100 refers to the
# power of each number i.e 20 Means 1 and 22 = 4
# so lets start

succes = False
NumberList = []
def iterator(Scale,Number):
    print("success")
    value = Scale ** Number
    rest = W - value
    print(rest)
    #  Now we need to find how we can match rest against the weight

    NumbeerList.append()

    



def CheckValues(weight,Scale):
    print(W)
    print(S)

    increment = 0
    #Now lets see if we can match the remaining number with power of scale

    while S != W:
        #Lets first decide if the weight is divisible by 2 without remainder so that
        #scale with the power of 0 can be ignored and continue with rest

        if (W % 2) == 0:
            #number is even number so lets start with 
            print("test")
        else:
            while succes != True:
                iterator(S,increment)
                increment = increment + 1
            









W = int(input("Enter weight :"))
S = int(input("Enter scale  :"))
CheckValues(W,S)







