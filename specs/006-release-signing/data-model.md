# Data Model: Release Signing

**Feature**: 006-release-signing

## Overview

This feature does not introduce new application data entities. It involves infrastructure configuration and artifact generation.

## Artifacts

### Keystore
- **File**: `singletask.keystore`
- **Location**: Repository Root
- **Format**: Java Keystore (JKS/PKCS12)
- **Content**: Private Key and Certificate for app signing.

### Build Output
- **File**: `com.companyname.singletask-Signed.aab` (Name depends on Package Name)
- **Location**: `src/SingleTask/bin/Release/net8.0-android/`
- **Format**: Android App Bundle
