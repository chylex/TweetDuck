#!/bin/bash

APPVERSION="$("$1TweetDuck" -appversion)"
LIBVERSION="$("$1TweetDuck" -pluginversion)"
INTRO="TweetDuck version $APPVERSION"

DASHES=$(seq 1 ${#INTRO})
printf "%0.s-" $DASHES
printf "\n$INTRO\nLibrary version $LIBVERSION\n"
printf "%0.s-" $DASHES
printf "\n"

find "$1plugins/" -type f -name ".meta" -exec echo "Processed {}" \; -exec sed -i "s/{version}/$LIBVERSION/" {} \;
