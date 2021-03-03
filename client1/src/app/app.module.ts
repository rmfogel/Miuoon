import { BrowserModule } from '@angular/platform-browser';
import { ErrorHandler, NgModule } from '@angular/core';
import { IonicApp, IonicErrorHandler, IonicModule } from 'ionic-angular';//:/umd
import { SplashScreen } from '@ionic-native/splash-screen';
import { StatusBar } from '@ionic-native/status-bar';
import { MyApp } from './app.component';
import { HomePage } from '../pages/home/home';
//import { HttpModule } from '@angular/http';
import { GooglePlacesAutocompleteComponentModule } from 'ionic2-google-places-autocomplete';
//import { FormsModule } from '@angular/forms';
// import { Hospitals } from '../pages/home/home.model1';
import { DataService, REST_URL } from '../Providers/dataService';
import { FilterPipe } from '../pages/home/home.hospitalPipe';
import {  HttpClientModule } from '../../node_modules/@angular/common/http';
import { FormsModule } from '../../node_modules/@angular/forms';
import { HttpModule } from '@angular/http';
//import { Hospitals2 } from '../pages/home/model2';
@NgModule({
  declarations: [
    MyApp,
    HomePage,
    FilterPipe,
   // Hospitals2
  ],
  imports: [
    BrowserModule,
    FormsModule,
    IonicModule.forRoot(MyApp),
    HttpModule,
    GooglePlacesAutocompleteComponentModule, 
    HttpClientModule    
  ],
  bootstrap: [IonicApp],
  entryComponents: [
    MyApp,
    HomePage
  ],
    providers: [
    DataService, 
    { provide: REST_URL, useValue: 'http://localhost:51037/api/hospital' },
    StatusBar,
    SplashScreen,
      { provide: ErrorHandler, useClass: IonicErrorHandler }, 
    
  ]
})
export class AppModule {}
