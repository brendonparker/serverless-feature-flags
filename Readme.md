# :fire: SFF: Serverless Feature Flags :fire:

This is a for-fun project to create a platform for managing feature flags (AKA "Feature Toggles").
For more information see: https://martinfowler.com/articles/feature-toggles.html

It is _Serverless_ because one of the main goals is for the platform to be deployable to AWS without needing to provision any servers.

## Dev Environment Requirements
- nodejs (18.x)
- dotnet SDK (6)
- cdk (`npm install -g aws-cdk@2.73.0`)
- an AWS account


## Build Process

This section describes the steps to build and deploy.

### Build Script
```
dotnet publish ./SFF.Lambdas -c Release -o ./LambdaSource/SFF.Lambdas
```

### Deploy
This will pickup the default AWS credentials/region configured for your dev environment

```
cdk deploy
```