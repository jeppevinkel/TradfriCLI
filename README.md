# TradfriCLI
A CLI application to interact with Ikea Tradfri gateways.

## Usage


| Command      | Arguments                               | Description.                                                                 |
|--------------|-----------------------------------------|------------------------------------------------------------------------------|
| devices      | -h \<host\> -p \<psk\> -c \<client id\> | Outputs a list of devices connected to the gateway.                          |
| generate-psk | -h \<host\> -p \<psk\> -c \<client id\> | Generates a new client PSK using the PSK found on the bottom of the gateway. |
