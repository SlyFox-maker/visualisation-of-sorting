arrHistory = []
def addChanged(array,i,x):
    ar=[];
    for w in range(len(array)):
        ar.append(array[w]);
    arrHistory.append([ar,i,x]);

def selectionSort(array):
    for i in range(len(array)-1):
        min_idx = i

        for idx in range(i + 1, len(array)-1):
            addChanged(array,i,idx)
            if array[idx] < array[min_idx]:
                min_idx = idx
        array[i], array[min_idx] = array[min_idx], array[i]
    return array


selectionSort(arr)
print(arrHistory)