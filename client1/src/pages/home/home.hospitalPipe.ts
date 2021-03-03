import { Pipe, PipeTransform } from '@angular/core';
import { CloseSearch } from './home.closeSearch';
import { ShortData } from './home.model1';

//סינון רשימת בתי חולים
@Pipe({
    name: 'filterHospitalList',
    pure: false
})
export class FilterPipe implements PipeTransform {
    public transform(hospitalList:ShortData[], search: string) {

         if (!search||hospitalList.find(i=>(i.Name)==search))
         {
            return [""]  
         }
       let filteredList:Array<ShortData>=new Array<ShortData>() 
       filteredList=hospitalList.filter(i => i.Name.includes(search));
       if (filteredList.length>0)
       return filteredList
       let max:number=0
       let temp:Array<number>=new Array<number>()
        hospitalList.forEach(h => 
        {
        temp.push(h.Name.length-CloseSearch.LevenshteinDistance(h.Name,search))
        });
      temp.forEach(i => 
      {
        if(i>max)
          max=i  
      })
           for(let i=0;i<temp.length;i++)
    {
        if(search.length>2&&temp[i]==max&&((search.length-max)/search.length)<0.5)
    {
        filteredList.push(hospitalList[i])
    }
}
    return filteredList
  }

 
}
