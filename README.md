# Deployr
Simple custom application deployment orchestration tool

#### Version 0.0.1
- Initial release of desktop runner app
- Send deploy commands from server to desktop runner and receive realtime logs


#### Internal Design
- `Processor`: Desktop application where it would receive deploy commands from `Deployr.Web`
- `Deployr.Web`: Server side that can send deploy scripts to the desktop runner
- `Web.Contracts`: Shared models and functionalities between desktop and server apps
