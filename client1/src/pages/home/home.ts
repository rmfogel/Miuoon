import { Component,Injectable, NgZone } from '@angular/core';
import { DataService } from '../../Providers/dataService';
import { ViewController, List } from 'ionic-angular';
import {  DepartmentInfo,ShortData, HospitalResalt } from './home.model1';
import { ActionSheetController } from 'ionic-angular';
import { FinalResulte } from './home.finalResulte';


declare var google: any
@Component({
  selector: 'page-home',
  templateUrl: 'home.html'
})
@Injectable()
 export class HomePage {
  address; 
  to_lat=0;
  to_lon=0;
  departmentList=new Array<ShortData>()
  departments=new Array<DepartmentInfo>();
  wings: Array<ShortData>
  hospitals:Array<ShortData>
  preferredHospital:ShortData=new ShortData;
  preferredDepartment:ShortData=new ShortData
  preferredWing: ShortData = new ShortData
  static startingPoint:string="";//ok -not implemented
  calculateHospitals= new Array<ShortData>(); 
  resultHospitals = new Array<HospitalResalt>(); 
  autocompleteItems;
  autocomplete;
  static latitude;
  static longitude; 
  static stringFormataddress: string;
  geo: any;
  service = new google.maps.places.AutocompleteService();

  constructor(private data: DataService, public viewCtrl: ViewController, private zone: NgZone, public actionSheetCtrl: ActionSheetController) 
  { 
    // this.data.getDataWithParam(4)
    ///////////delete

    this.address = {
      place: ''
    };
    this.autocompleteItems = [];
    this.autocomplete = {
      query: ''
    };   
    this.data.getData().subscribe(dt => {
      var d = JSON.parse(dt.toString());
  //חילוץ בתי החולים שנשלחו מהשרת 
  d.forEach( h1 => this.departments.push(h1)); });  
    this.preferredHospital
   
   } 
 //איפוס הכתובת
  dismiss() {
    this.viewCtrl.dismiss()
  }
 //בחירת הכתובת
  chooseItem(item: any) {
    // this.viewCtrl.dismiss(item);
    this.autocomplete.query = item
    this.geo = item;
    this.geoCode(this.geo);//convert Address to lat and long
    HomePage.startingPoint=item
    this.autocompleteItems = [];
  }
 //עידכון הכתובת
  updateSearch() 
  {

    if (this.autocomplete.query == '') {
      this.autocompleteItems = [];
      return;
    }

    let me = this;
    this.service.getPlacePredictions({
      input: this.autocomplete.query,
      // componentRestrictions: {
      //     country: 'de'
      // }
    }, (predictions, status) => {
      me.autocompleteItems = [];

      me.zone.run(() => {
        if (predictions != null) {
          predictions.forEach((prediction) => {
            me.autocompleteItems.push(prediction.description);
          });
        }
      });
    });
  }

  //convert Address string to lat and long
    geoCode(address: any,isDest:boolean=false) {
    let geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'address': address }, (results, status) => {
      if(isDest==false)
      {
    HomePage.latitude = results[0].geometry.location.lat();
    HomePage.longitude = results[0].geometry.location.lng();
      }
      else
      {
    this.to_lat = results[0].geometry.location.lat();
    this.to_lon = results[0].geometry.location.lng();
      }
    });
  }

 
 //קבלת מיקום נוכחי
 static getGeoLocation(lat: number, lng: number) {
  

 var geocoder = new google.maps.Geocoder();
 var latlng = {"lat": lat, "lng": lng};
    
//var latlng = new google.maps.LatLng(31.000000,34.980007);

let request = {
  location: latlng 
};
geocoder.geocode({'latLng': latlng}, (results, status) => {
  if (status == google.maps.GeocoderStatus.OK) 
  {
    console.log(results)
    if (results[0] != null) 
    {
      HomePage.startingPoint = results[0].formatted_address;
console.log(HomePage.startingPoint)
     
    } else {
      alert("No address available");
    }
  }
  else alert(status)
});
  }

 //קבלת מיקום
  getLocation()
  {
    var options = {
      enableHighAccuracy: true,
      timeout: 5000,
      maximumAge: 0
    };
    
   
     function success(pos) {
      var crd = pos.coords;
      if(crd!=null)
      {
         console.log(`Latitude : ${crd.latitude}`);
         console.log('Your current position is:');
         console.log(`Longitude: ${crd.longitude}`);
         console.log(`More or less ${crd.accuracy} meters.`);
         if(HomePage!=null)
         {
        HomePage.longitude = crd.longitude
        HomePage.latitude = crd.latitude
         }
         HomePage.getGeoLocation(crd.latitude,crd.longitude)
         
      }
     }

    function error(err) {
      console.warn(`ERROR(${err.code}): ${err.message}`);
    }
   
    navigator.geolocation.getCurrentPosition(success, error, options);
  
   

  }

 //קבלת רשימת בתי חולים 
  getHospitalNames()
  {
    this.calculateHospitals=null
    this.calculateHospitals=new Array<ShortData>()
    this.departments.forEach(element => {
      var t = new ShortData();
    t.Code=element.HospitalCode
    t.Name=element.HospitalName
          if (this.calculateHospitals.find(i => i.Code == t.Code)==null)
              this.calculateHospitals.push(t)
    });
    
    return this.calculateHospitals
  }

  //בחירת בית חולים רצוי
  setPerferredHospital(selection:ShortData)
  {
    this.preferredHospital.Code=selection.Code
    this.preferredHospital.Name=selection.Name
  }

 //קבלת רשימת מחלקות
  getDepartmentList() 
  {
    this.departmentList = null
    this.departmentList = new Array<ShortData>()
    this.departments.forEach(element => {
    if(element.HospitalCode==this.preferredHospital.Code&&element.BaseDepartmentCode==null)
    {  var t = new ShortData();
      t.Code = element.DepartmentCode
      t.Name = element.DepartmentTypeName
        this.departmentList.push(t)
    }
    }); 
     return this.departmentList
  }

 //קבלת רשימת סוגי מיון
  getWingList()
  {
    this.wings = null
    this.wings = new Array<ShortData>()
    if (this.preferredDepartment.Code != null)
    this.departments.forEach(element => {
      if (element.HospitalCode == this.preferredHospital.Code && element.BaseDepartmentCode == this.preferredDepartment.Code) {
        var t = new ShortData();
        t.Code = element.DepartmentCode
        t.Name = element.DepartmentTypeName
        this.wings.push(t)
      }
    });
    return this.wings
  }

 
 async getHospitalsPList(){
    this.data.getHospiyalPreferedList(HomePage.startingPoint, this.preferredHospital.Code,
      this.preferredDepartment.Code, this.preferredWing.Code).subscribe(dt => {
        this.resultHospitals = JSON.parse(dt.toString())
      }); 
  }
  
  presentResult() {
    var space = '\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0'
   if(HomePage.startingPoint=="")
    this.getLocation()
    const actionSheet = this.actionSheetCtrl.create({
      title: `${space}בתי החולים המומלצים- לחץ לניווט`,
    });
    if(this.preferredHospital==undefined)
    {
    this.preferredHospital=new ShortData();
    this.preferredHospital.Code=-1; 
    this.preferredHospital.Name=""
    }
    if (this.preferredDepartment == undefined) {
      this.preferredDepartment = new ShortData();
      this.preferredDepartment.Code = -1;
      this.preferredDepartment.Name="";
    }
    if (this.preferredWing == undefined) {
      this.preferredWing = new ShortData();
      this.preferredWing.Code = -1;
      this.preferredWing.Name=""
    }
    this.getHospitalsPList()
   

     console.log(this.resultHospitals)
    //  for (var property in this.resultHospitals) {
    //   console.log(property, '= ', JSON.stringify(this.resultHospitals[property]));
    // }
//todo async!!!!
if(this.resultHospitals!=null)
{
   this.resultHospitals.sort(h =>h!=null&& h.TravelingTime+h.WaitingTime)
   console.log("1111111")
  console.log(  this.resultHospitals.find(h =>h!=null&& h.IsPreferred == true))
if(this.resultHospitals.find(h =>h!=null&& h.IsPreferred == true)==undefined)
   this.resultHospitals[2] = null;
 else  this.resultHospitals[2] =this.resultHospitals.find(h => h.IsPreferred == true)
  if (this.resultHospitals[2]!=null&&this.resultHospitals[0].HospitalCode == this.resultHospitals[2].HospitalCode)
    this.resultHospitals[0]=null

  if (this.resultHospitals[2] !=null&&this.resultHospitals[1].HospitalCode == this.resultHospitals[2].HospitalCode)
      this.resultHospitals[1] = null
     space = '\xa0\xa0\xa0\xa0'//7
     // // icon: 'ion-android-favorite-outline', 

    // this.resultHospitals.forEach(h => {
      if(this.resultHospitals[2]!=null)
      actionSheet.addButton(
        {
          text: `${this.resultHospitals[2].HospitalName}${space}תוך${space}${this.resultHospitals[2].TravelingTime + this.resultHospitals[2].WaitingTime}${space} דקות `,
          role: 'destructive',
          icon: 'heart', 
          handler: () => {
            this.geoCode(this.resultHospitals[2].HospitalAdrress,true)
            this.redirect()
          }
        })
    if (this.resultHospitals[0]!=null)
         actionSheet.addButton(
          {
             text: `${this.resultHospitals[0].HospitalName}${space}תוך${space}${this.resultHospitals[0].TravelingTime + this.resultHospitals[0].WaitingTime}${space} דקות `,
            role: 'destructive',
             icon: 'stopwatch',
            handler: () => {
              this.geoCode(this.resultHospitals[0].HospitalAdrress, true)
              this.redirect()
            }
          }
      )
    if (this.resultHospitals[1] != null)
      actionSheet.addButton(
        {
          text: `${space}${space}${space}\xa0\xa0${this.resultHospitals[1].HospitalName}${space}תוך${space}${this.resultHospitals[1].TravelingTime + this.resultHospitals[1].WaitingTime} ${space}דקות `,
          role: 'destructive',
          handler: () => {
            this.geoCode(this.resultHospitals[1].HospitalAdrress, true)
            this.redirect()
          }
        }
      )
    }
    // });
  

    actionSheet.present();
    console.log(this.resultHospitals)
  }

  //קישור למפה
  redirect() {
    // this.getLocation()

    window.open(`https://www.waze.com/he/livemap?at=now&from_lat=${HomePage.latitude}&from_lon=${HomePage.longitude}1&to_lat=${this.to_lat}&to_lon=${this.to_lat}`);
  }
 

}
