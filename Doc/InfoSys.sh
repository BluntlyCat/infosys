#! /bin/bash

BROWSER="TYPE_YOUR_BROWSER_HERE"

if [ "$1" != "" ] ; then
    BROWSER="$1"
fi

if [ "$BROWSER" != "TYPE_YOUR_BROWSER_HERE" ] ; then
    $BROWSER html/index.html
else
    echo "Please specify your browser in 'BROWSER' variable at the top of this"
    echo "file or run this script by passing your browser as argument."
    printf "\nType for example:"
    printf "\n\t./InfoSys firefox\n\n"
    echo "for watching the documentation in firefox"
fi

exit 0
