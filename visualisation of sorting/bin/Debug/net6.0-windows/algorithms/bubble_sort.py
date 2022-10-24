# arr - array for sorting
# arrHistory[step]="First changed element-Second changed element" - array of history
# you need add function about write history of change of array for send that to C# script and do a animation
arrHistory = []
def addChanged(array,i,x):
    ar=[];
    for w in range(len(array)):
        ar.append(array[w]);
    arrHistory.append([ar,i,x]);

def bubbleSort(array):
  # loop to access each array element
  for i in range(len(array)):

    # loop to compare array elements
    for j in range(0, len(array) - i - 1):
        addChanged(array,j,j+1)
        # compare two adjacent elements
        # change > to < to sort in descending order
        if array[j] > array[j + 1]:
            # swapping elements if elements
            # are not in the intended order
            temp = array[j]
            array[j] = array[j+1]
            array[j+1] = temp

bubbleSort(arr)
print(arrHistory)