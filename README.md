# MyPass

[![asciicast](https://asciinema.org/a/3HQWgen5KK3GSctDnaM8ohQik.svg)](https://asciinema.org/a/3HQWgen5KK3GSctDnaM8ohQik)

MyPass is a terminal-based password manager inspired by UNIX.
You use it by running commands, and piping their output
into one-another.

## Commands

### `ls`

Lists all logins

### `help [command]`

Displays help message for `command`, or general help

### `set-password`

Prompts user for the master password.

### `open <file>`

Opens a file, decrypting it with the master password.

### `save [file]`

Saves to `file`, or the last place it saved otherwise.

### `find [--for <for>] [--username <username>] [--tag <tag>]+`

Finds logins that match the given criteria.

### `add [--for <for>] [--username <username>] [--password <password>]`

Adds the given login, prompting the user for a password if one
was not specified.

### `rm [--for <for>] [--username <username>] [--tag <tag>]+`

Removes all logins that match the given criteria.

### `password`, `username`, `tags`, `notes`

Gets the appropriate field from the logins piped into it.
For example:

```sh
-> find -for gmail | password
```

## Building

On Linux, the `install_linux.sh` script can be used to build
and install MyPass. On Windows, build it with Visual Studio.
On OS X go install Linux.
