import { Component } from '@angular/core';
import { Platform } from 'ionic-angular';
import { StatusBar } from '@ionic-native/status-bar';
import { SplashScreen } from '@ionic-native/splash-screen';
import { HomePage } from '../pages/home/home';
// import {  Injectable } from '@angular/core';
// import { DataService } from './dataService';
@Component({
  templateUrl: 'app.html'
})

// @Injectable()
// export class AppComponent {
//   model = new Model();
//   title = 'app';
//   constructor(private data: DataService) {
//     this.data.getData().subscribe(dt => this.title = dt);
//   }

export class MyApp {
  rootPage:any = HomePage;

  constructor(platform: Platform, statusBar: StatusBar, splashScreen: SplashScreen) {
    platform.ready().then(() => {
      // Okay, so the platform is ready and our plugins are available.
      // Here you can do any higher level native things you might need.
      statusBar.styleDefault();
      splashScreen.hide();
    });
  }
}

