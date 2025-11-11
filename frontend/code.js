let i = 1.0; 
let date = new Date();
const { createElement } = require("react");

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
function newpost(name, stars, text, tmdb_id, tmbd_name){
    

    const div = document.createElement("div");
    div.className = "post";
    div.id = i;
    document.getElementById("posts_container").appendChild(div);
        
    //img
    const img = document.createElement("img");
    img.src = "noimage.png";
    img.className = "poster";
    document.getElementById(i).appendChild(img);

    // post_text
        const post_text = document.createElement("div");
        post_text.className = "post_text";
        post_text.id = "text" + i;
        document.getElementById(i).appendChild(post_text);

        const author_span = document.createElement("span");
        author_span.className = "author_date";
        author_span.innerHTML = "Пише " + name + ", " + date.toLocaleDateString() + ",";
        document.getElementById(i).getElementsByClassName("post_text")[0].appendChild(author_span);

        const star_view = document.createElement("div");
        star_view.className = "stars";
        document.getElementById(i).getElementsByClassName("post_text")[0].appendChild(star_view);

       

        for(let a = 0.0; a < stars; ++a){
            const star_image = document.createElement("img");
            star_image.src = "star.svg";
            document.getElementById(i).getElementsByClassName("stars")[0].appendChild(star_image);

        }
        star_view.innerHTML += "(" + stars + ")";
        
        const innerpost = document.createElement("p");
        innerpost.innerHTML = text;
        document.getElementById(i).getElementsByClassName("post_text")[0].appendChild(innerpost);
        
        const talking_about = document.createElement("span");
        talking_about.innerHTML = "Йдеться про ";
        talking_about.id = tmdb_id + "_" + i;
        document.getElementById(i).getElementsByClassName("post_text")[0].appendChild(talking_about);

        const movie = document.createElement("a");
        movie.href = "https://themoviedb.org/" + tmdb_id + "/";
        movie.innerHTML = tmbd_name;
        document.getElementById(tmdb_id+"_"+i).appendChild(movie);
    i++;
}