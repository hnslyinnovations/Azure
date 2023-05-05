$MaxReturn = 15000
#Please modify the following properties
$resourceGroup = ""
$storageAccountName = ""
$containerName = ""
$FullPath = ""
#ignore fullpath variable you want to get total size of container opposed to path along storage
#End Modifications
 
$Total = 0
$Token = $Null
$thesum = 0
 
$storageAccount = Get-AzStorageAccount -ResourceGroupName $resourceGroup -Name $storageAccountName 
 
$ctx = $storageAccount.Context 	
 
do
{
     $Blobs = Get-AzStorageBlob -Container $ContainerName -MaxCount $MaxReturn -Prefix $FullPath -ContinuationToken $Token -Context $ctx
     $Total += $Blobs.Count
     if($Blobs.Length -le 0) { Break;}
     $Token = $Blobs[$blobs.Count -1].ContinuationToken;
     $temp = $Blobs | Measure-Object -property length -sum
     $thesum =  $thesum + $temp.Sum
}
While ($Token -ne $Null)

$totalsize = [math]::ceiling((($thesum/1024)/1024))
Write-Host " "
Write-Host "++++++++++++++++++++++++++++++"
Write-Host " "
Echo "Total $Total blobs in container $ContainerName"
Write-Host " "
Write-Host "The size of $FullPath is = " $totalsize "MB"
Write-Host "-----All Property-----"