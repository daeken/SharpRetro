// version.h

#ifndef VERSION_H
#define VERSION_H

#define APP_NAME "b64"

#define VERSION_MAJOR 1
#define VERSION_MINOR 0
#define VERSION_PATCH 0

// Pre-release and build metadata
#define VERSION_PRERELEASE "alpha"
#define VERSION_BUILD "20250422"

// Stringification helpers
#define STRINGIFY(x) #x
#define TOSTRING(x) STRINGIFY(x)

// Core version string: "1.0.3"
#define VERSION_CORE_STR TOSTRING(VERSION_MAJOR) "." TOSTRING(VERSION_MINOR) "." TOSTRING(VERSION_PATCH)

// Full version with optional pre-release and build
#ifdef VERSION_PRERELEASE
  #define VERSION_FULL_STR VERSION_CORE_STR "-" VERSION_PRERELEASE
#else
  #define VERSION_FULL_STR VERSION_CORE_STR
#endif

#ifdef VERSION_BUILD
  #define VERSION_STR VERSION_FULL_STR "+" VERSION_BUILD
#else
  #define VERSION_STR VERSION_FULL_STR
#endif

#endif // VERSION_H
