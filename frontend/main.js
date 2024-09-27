window.addEventListener('DOMContentLoaded', (event) =>{
    getVisitCount();
})

const productionApiUrl = "https://getresumecounterr.azurewebsites.net/api/GetResumeCounter?code=96Vv91wgQ15Ni3otJXWODMCVBigqHxT_cnUbKhF9UxZCAzFu3Elxjw%3D%3D";
const localfunctionApi = "http://localhost:7071/api/GetResumeCounter";

const getVisitCount = () => {
    let count = 30;
    fetch(productionApiUrl).then(response => {
        return response.json()
    }).then(response =>{
        console.log("Website called function API.");
        count =  response.count;
        document.getElementById("counter").innerText = count;
    }).catch(function(error){
        console.log(error);
    });
    return count;
}