	// import { Http } from '@angular/http';
    // import {  Response, Headers } from '@angular/http';
	import { Injectable, Inject, InjectionToken } from '@angular/core';
	import 'rxjs/add/operator/map';
	import { Observable } from 'rxjs/Observable';
	import { ShortData } from '../pages/home/home.model1'
	import { HttpClient, HttpHeaders, HttpParams, HttpClientModule} from '@angular/common/http';
import { observableToBeFn } from 'rxjs/testing/TestScheduler';
	
	export const REST_URL = new InjectionToken('rest_url');
	const httpOptions = {     headers: new HttpHeaders({'Content-Type':  'application/json'})}; 
	@Injectable()
	export class DataService {
	  url='http://localhost:51037/api/hospital'
	
		 constructor(private http: HttpClient) {} 
		getData(): Observable<string> {
			return this.http.get<string>(this.url) }


			// getDataWithParam(str:any, num1:number, num2:number, num3:number):Observable<string> 
			// {
			// 	const url2 = `${this.url}/suitable/${str}/${num1}/${num2}/${num3}`;

			// 	console.log(url2)
			// 	 this.http.get<string>(url2).subscribe(
			// 			data=>{console.log(data)
			// 			   },error=>{console.log(error)
			// 			   });

			// 			   return;
			// }

		getHospiyalPreferedList(location: string, preferedHospital: number, preferedDepartment: number, preferedSubDepartmenrt: number): Observable<string> {
			if(preferedHospital==null)
			preferedHospital=-1;
			if (preferedDepartment == null)
				preferedDepartment = -1;
			if (preferedSubDepartmenrt == null)
				preferedSubDepartmenrt = -1;
			console.log(location)	
				// location="המכללה הטכנולוגית באר שבע"
			const url2 = `${this.url}/suitable/${location}/${preferedHospital}/${preferedDepartment}/${preferedSubDepartmenrt}`;

			console.log(url2)
			
		return	this.http.get<string>(url2);

	      	}

 
			
			// getDataWithParam(num3:number):Observable<string>
			// {
			// 	const url2 = `http://localhost:49508/api/Default/5`; 

			// 	console.log(url2)
			// 	this.http.get<string>(url2).subscribe(
			// 		data=>{console.log(data)
			// 	   },error=>{console.log(error)
			// 	   });
			// 	   return ;
				
			// }
			
		 saveData(hospitalCode:number ): Observable<ShortData>   
		  {
			let params = new HttpParams().set("paramName", hospitalCode.toString())//Create new HttpParams
			console.log("post "+ hospitalCode)
			 return this.http.get<ShortData>(this.url, {params:params});
		  }


		  


		//The last code:
		//  constructor(private http: Http, @Inject(REST_URL) private url: string) 
		// 	 {}
      	// 	  getData(): Observable<string> 
		// 	  {
        // 	  return this.http.get(this.url).map(respo => respo.json());
        //       } 
	
// 	postData(message: string):Observable<string>{
// return this.http.post(this.url,message).map(respo=>respo.json());
// 	}
    
}
