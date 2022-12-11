#!/bin/bash
# This is a dummy script only. It does nothing except output information.
# Ensure that it has executable permission and specify the POSTPUBLISH config parameter to point at it.

echo "DUMMY POSTPUBLISH SCRIPT"
echo "APPMAIN: ${APPMAIN}"
echo "APPID: ${APPID}"
echo "APPVERSION: ${APPVERSION}"
echo "VERSION: ${VERSION}"

echo "ISODATE: ${ISODATE}"
echo "DOTNETRID: ${DOTNETRID}"
echo "PKGKIND: ${PKGKIND}"
echo "APPDIRROOT: ${APPDIRROOT}"
echo "APPDIRUSR: ${APPDIRUSR}"
echo "APPDIRBIN: ${APPDIRBIN}"
echo "APPRUNTARGET: ${APPRUNTARGET}"

cp -r Assets/icons/hicolor $APPDIRUSR/share/icons
