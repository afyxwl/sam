let i = 1.0; 
let date = new Date();
const { createElement } = require("react");

function myfunc(){
    console.log("rizz");
}
function post(){
    const xhr = new XMLHttpRequest();
        xhr.open("POST", "http://localhost:5096/posts/");
        xhr
        .setRequestHeader("Content-Type", "application/json");
        const body = JSON
        .stringify(
        {
            userId: 1,
            title: "Demo Todo Data using XMLHttpRequest",
            completed: false,
        });
        xhr
        .onload = () => 
        {
            if (xhr.readyState == 4 && xhr.status == 201) 
            {
                console.log(JSON.parse(xhr.responseText));
            } else 
            {
                console.log(`Error: ${xhr.status}`);
            }
        };
        xhr.send(body);
}
function getall(){
<<<<<<< HEAD
    fetch("http://localhost:5096/reviews/",{
=======
    fetch("http://10.216.208.288:5096/reviews/",{
>>>>>>> 024b771 (?)
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

function buttonPress(){
    newpost(
        document.getElementById("iname").value,
        document.getElementById("istar").value,
        document.getElementById("ipost").value,
        document.getElementById("itmdb").value,
        "unknown"
    );
}
