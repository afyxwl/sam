function myfunc(){
    console.log("rizz");
}
function post(){
    fetch("http://localhost:5041/users/",{
        method: "POST",
        body: JSON.stringify({
            name: "Persin",
            group: "PI-271"
        }),
        headers: {
    "Content-type": "application/json; charset=UTF-8",
    'Access-Control-Allow-Origin':'*',
    'Access-Control-Allow-Methods':'POST,PATCH,OPTIONS'
        }
    })
    .then((response) => response.json())
    .then((json) => console.log(json));
}
function getall(){
    fetch("http://localhost:5041/users/",{
        method: "GET",
        headers: {
    "Content-type": "application/json; charset=UTF-8"}
    })
    .then((response) => response.json())
    .then((json) => console.log(json));
}