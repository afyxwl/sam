let server = "http://localhost:5096/";

let i = 1.0; 
let date = new Date();
//const { createElement } = require("react");

function myfunc(){
    console.log("rizz");
}
function post(text, name, tmdbID,stars){
    try{
    var get = fetch(server+"reviews", {
        method: "POST",
        headers: {
            "Content-type": "application/json; charset=UTF-8"
        },
        body: JSON.stringify({displayName: name, text: text, tmdbId: tmdbID,rating: stars}),
    })
    ;
    return get
        .then((response) => {
            if (response.status === 200) return 200;
            if (response.status === 400) return 400;
            if (response.status === 404) return 404;
            return response.status;
        })
        .catch((error) => { console.error(error); throw error; });
}
    catch(error){alert(error.message);}
}
function getall(){
  //  try{
  var a;
    fetch(server + "reviews", {
        method: "GET",
        headers: {
            "Content-type": "application/json; charset=UTF-8"
        }
        
    })
    .then((response) => response.json())
    .then((data) => {
        
        for(let i = 0.0; i < data.length; i++){
            var dto = data[i];
            const dtodate = new Date(data[i].createdAt);
            newpost(dto.displayName,dto.rating,dto.text,dto.tmdbId,dto.movieName,dto.posterUrl,dtodate.toLocaleString("uk-UA",{timezone: "UTC+3"}));
        }

    })
    .catch((error) => {
        console.log("error", error);
    });
    
//}
   // catch(error){console.log("error");}   
    //var a = json.Parse();
    //for(let i = 0; i < a.length; i++){
    //    newpost(a[i].displayName,a[i].rating,a[i].text,a[i].tmdbID,a[i]);
    //}
}
function newpost(name, stars, text, tmdb_id, tmbd_name,image_url, created_at){
    
    if(stars > 10 || stars < 1){ alert("Кількість зірочок підтримується у значеннях від 1 до 10."); return;}
    if(!name){ alert("Ім'я пусте."); return;}
    if(!text){ alert("Текст посту пустий."); return;}
    if(!tmdb_id){ alert("Поле з tmdb ID пусте."); return;}


    const div = document.createElement("div");
    div.className = "post";
    div.id = i;
    document.getElementById("posts_container").appendChild(div);
        
    //img
    const img = document.createElement("img");
    img.src = image_url;
    img.className = "poster";
    document.getElementById(i).appendChild(img);

    // post_text
        const post_text = document.createElement("div");
        post_text.className = "post_text";
        post_text.id = "text" + i;
        document.getElementById(i).appendChild(post_text);

        const author_span = document.createElement("span");
        author_span.className = "author_date";
        author_span.innerHTML = "Пише " + name + ", " + created_at + ",";
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
        movie.href = "https://themoviedb.org/movie/" + tmdb_id + "/";
        movie.innerHTML = tmbd_name;
        document.getElementById(tmdb_id+"_"+i).appendChild(movie);
    i++;
}

function buttonPress(){
    try{
    var a = post(
        document.getElementById("ipost").value,
        document.getElementById("iname").value,
        document.getElementById("itmdb").value,
        document.getElementById("istar").value,
    );
    a.then((status) => {
        if (status === 400 || status === 404) {
            alert("Відбулась помилка. Спробуйте інший запит.\nКод помилки: " + status); return;
        }
        else{
    alert("Пост опубліковано. \nНатисніть щоб перезавантажити сторінку.")

            location.reload();
        }
    }).catch((error) => {
        console.error(error);
    });
    
    
}
    catch(error){ alert(error);}
}
