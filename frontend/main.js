window.addEventListener('DOMContentLoaded', (event) =>{
    getVisitCount();
})

const productionApiUrl = "https://azure-resume120241001210159.azurewebsites.net/api/GetResumeCounter?code=t1UmBQcnyQ94J8OjKvbNYer_oF-hR15qTOs4N8l6Bf9iAzFuiUpMYA%3D%3D";
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