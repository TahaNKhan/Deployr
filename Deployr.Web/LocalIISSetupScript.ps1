Clear-Host

Write-Host "Setting up Deployr.Web Service for local development..."
Write-Host "---------------------------------------------------"
Write-Host "DO NOT USE THIS FOR PRODUCTION"


$isIISEnabled = Get-Service W3SVC -ErrorAction:SilentlyContinue
if (!$isIISEnabled) {
	Write-Host "Enabling IIS"
	Enable-WindowsOptionalFeature -online -featurename IIS-WebServerRole
}

$localUserName = "Deployr_Web_Runner"
$localUserPassword = "2VXjYXwFDN"

$localUserExists = (Get-LocalUser Deployr_Web_Runner -ErrorAction:SilentlyContinue).Enabled

if (!$localUserExists) {
	New-LocalUser $localUserName -Password $localUserPassword | Out-Null
}

# IIS Stuff
Import-Module WebAdministration

$appPoolName = "Deployr.Web"

$appPool = Get-IISAppPool $appPoolName

if (!$appPool) {
	Write-Host "IIS App Pool does not exist, creating..."
	$appPool = New-WebAppPool -Name $appPoolName -Force
	Write-Host "Done..."
}

# No managed code
Set-ItemProperty IIS:\AppPools\$appPoolName managedRuntimeVersion ""

$currentPath = (pwd).Path;

# TODO Setup site in IIS

# TODO Setup site to run as created user
