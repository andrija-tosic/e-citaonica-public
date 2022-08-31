// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  apiEndpoint : 'https://localhost:7246',
  msalConfig: {
    auth: {
      clientId : 'ed3ac4f0-6e54-409a-8386-f5971779ff3b',
      redirectUri : 'http://localhost:4200/login/',
      authority: 'https://login.microsoftonline.com/f5501f2a-b2b9-4122-8f2d-aa0f0366d907'
    }
  }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
