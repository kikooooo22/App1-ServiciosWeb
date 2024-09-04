import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient() // Permite inyectar angular en diferentes partes 
    /* 
    Todos soportan inyeccion de dependencias (angular)
    No instanciar una clase directamente, sino se le dice al codigo 
    que naturalmente debe incluirlo 
    Se basa en los 5 'SOLID Principles'  
    Esto es bueno pues el codigo es mas independiente, mas mantenible, etc.
    */
  ]
};
