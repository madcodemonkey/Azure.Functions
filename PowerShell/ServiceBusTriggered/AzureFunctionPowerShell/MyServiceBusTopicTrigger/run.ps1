# Helpful link about problems with input parameters: https://github.com/Azure/azure-functions-powershell-worker/issues/531

param($mySbMsg, $TriggerMetadata)

Write-Host "Yates!  PowerShell ServiceBus topic trigger!!"

Write-Host "Yates! Message: $mySbMsg"
Write-Host "Yates! Metadata: $TriggerMetadata"

Write-Host "---mySbMsg---"
Write-Host ($mySbMsg | ConvertTo-Json -Compress)  # Example output: {"Name":"George Carlin1","Url":"https://www.google.com"}
Write-Host "Name: $($mySbMsg.Name)"               # Example output: Name: George Carlin1  
Write-Host "Url: $($mySbMsg.Url)"                 # Example output: Url: https://www.google.com
Write-Host "WebUrl: $($mySbMsg.WebUrl)"  # Blank because this was a special property sent to and used only by the service bus

Write-Host "--------Meta------------------"
Write-Host "---TriggerMetaData---"
$TriggerMetadata.keys | % { Write-Host "$_" }