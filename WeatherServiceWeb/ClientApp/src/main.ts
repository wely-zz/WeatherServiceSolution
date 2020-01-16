import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

export function getBaseUrl() {
  return document.getElementsByTagName('base')[0].href;
}
export function getLocalFunctionUrl() {
  return 'http://localhost:7071/api/';
}
export function getAzureFunctionUrl() {
  return 'https://virtualsummitappcs.azurewebsites.net/api/';
}


const providers = [
  { provide: 'BASE_URL', useFactory: getBaseUrl, deps: [] },
  { provide: 'LOCAL_FUNCTION', useFactory: getLocalFunctionUrl, deps: [] },
  { provide: 'AZURE_FUNCTION', useFactory: getAzureFunctionUrl, deps: [] }
];

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic(providers).bootstrapModule(AppModule)
  .catch(err => console.log(err));
