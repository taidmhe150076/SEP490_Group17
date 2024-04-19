var myModal = new bootstrap.Modal(document.getElementById("exampleModal"));
$(() => {
    console.log("first"+ url_single)
    // Show the modal when the page loads
    display_survey(url_list);
});

var use = `
<div class="row">
        <div class="col">
            <h2>Series Name</h2>
            <ul class="list-group">
                <li class="list-group-item">
                    WorkshopSeriesLab318
                    <h3>Workshop</h3>
                    <ul class="list-group">
                        <li class="list-group-item">
                            Docker Technology: Evolution and Cl/CD practice
                            <h4>Survey</h4>
                            <ul class="list-group">
                                <li class="list-group-item"><a href="https://docs.google.com/forms/d/e/1FAIpQLSc0lY870e9i8HqarA_6lPEVUyaO3QG2W0eEazHXOUyUBMSO7g/viewform">presenter form</a></li>
                            </ul>
                        </li>
                    </ul>
                </li>
            </ul>
        </div>
    </div>
`;

var display_survey = (url) => {
    myModal.show();
    $.get(url)
        .done((res) => {
            var row_list = "";
            setTimeout(function () {
                myModal.hide();
            }, 1000);
            res.forEach((wss) => {
                row_list += `
              <div class='row'>
                <div class="col">
                    <h2>Series Name: ${wss.workshopSeriesName}</h2>
                        <ul class="list-group">
                            <li class="list-group-item">
                                <h3>Workshop</h3>
                                <ul class="list-group">
                                    <li class="list-group-item">
                                        ${workshop_gen(wss.workshops, wss.id)}
                                    </li>
                                </ul>
                            </li>
                        </ul>
                </div>
              </div>
            `;
            });
            $("#app-content").append(row_list);
        })

        .fail((err) => {
            console.log(err);
        });
};

var workshop_gen = (workshops, id) => {
    let workshop_list = "";
    workshops.forEach((ws) => {
        workshop_list += `
            <h3> workshop name :${ws.workshopName}</h3>
            <h5>Survey</h5>
            <ul class="list-group">
                ${survey_gen(ws.survey, id, ws.id)}
            </ul>
        `;
    });
    return workshop_list;
};

var survey_gen = (surveys, wssid, wsid) => {
    var survey_list = "";
    if (surveys.length === 0) {
        survey_list += `<li class="list-group-item mt-2">
                            <div class="row">
                                <div class='col-md-6'> Add Survey?</div>
                                <div class='col-md-6'>
                                    <form action=${url_add} method="get">
                                        <input type="hidden" name="wssId" value="${wssid}">
                                        <input type="hidden" name="wsId" value="${wsid}">
                                        <button type="submit" class="btn btn-primary">Add Survey</button>
                                    </form>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-12">
                                    Workshop dont have any survey yet
                                </div>
                            </div>
                        </li>`;
    } else {
        var table = `
            <li class="list-group-item mt-2">
                <div class="row">
                    <div class='col-md-6'> Add Survey?</div>
                    <div class='col-md-6'>
                    <form action=${url_add} method="get">
                        <input type="hidden" name="wssId" value="${wssid}">
                        <input type="hidden" name="wsId" value="${wsid}">
                        <button type="submit" class="btn btn-primary">Add Survey</button>
                    </form>
                    </div>
                </div>
                <table class='table'>
                    <thead>
                        <tr>
                            <th>Survey Name</th>
                            <th>Form url</th>
                            <th>Form type</th>
                            <th>Action</th>
                        </tr>
                    </thead>
                    <tbody>
                       {contents}
                    </tbody>
                </table>
            </li>
        `;
        table_content = ''
        surveys.forEach((survey) => {
            var action = "";
            if (survey.survey_path == null) {
                action = `
                        <form action=${url_update} method="get">
                        <input type="hidden" name="wssId" value="${wssid}">
                        <input type="hidden" name="wsId" value="${wsid}">
                        <input type="hidden" name="surveyId" value="${survey.id}">
                        <button type="submit" class="btn btn-warning">Submit survey response</button>
                    </form>
                    <form action=${url_delete} method="post">
                                        ${token}
                                        <input type="hidden" name="wssId" value="${wssid}">
                                        <input type="hidden" name="wsId" value="${wsid}">
                                        <input type="hidden" name="surveyId" value="${survey.id}">
                                        <button type="submit" class="btn btn-danger">Delete Survey</button>
                    </form>
                `
            } else {
                action = `
                    <form action=${url_single} method="get">
                        <input type="hidden" name="wssId" value="${wssid}">
                        <input type="hidden" name="wsId" value="${wsid}">
                        <input type="hidden" name="surveyId" value="${survey.id}">
                        <button type="submit" class="btn btn-primary">View survey response</button>
                    </form>
                    <form action=${url_update} method="get">
                        <input type="hidden" name="wssId" value="${wssid}">
                        <input type="hidden" name="wsId" value="${wsid}">
                        <input type="hidden" name="surveyId" value="${survey.id}">
                        <button type="submit" class="btn btn-success">Update survey response</button>
                    </form>
                    <form action=${url_delete} method="post">
                                        ${token}
                                        <input type="hidden" name="wssId" value="${wssid}">
                                        <input type="hidden" name="wsId" value="${wsid}">
                                        <input type="hidden" name="surveyId" value="${survey.id}">
                                        <button type="submit" class="btn btn-danger">Delete Survey</button>
                                    </form>
                    
                `
            }
            table_content += `
            <tr>
                <td>${survey.survey_name}</td>
                <td> <a href='${survey.survey_form}' target="_blank" >survey form </a></td>
                <td>${survey.isPresenter}</td>
                <td>${action}</td>
            </tr>
            `;
        });
        table = table.replace("{contents}", table_content);
        survey_list += table;
    }
    return survey_list;
};

var deleteItem = (wssId, wsId, surveyId) => {
    $.ajax({
        url: url_delete,
        type: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token // include the anti-forgery token in the request headers
        },
        data: {
            wssId: wssId,
            wsId: wsId,
            surveyId: surveyId
        },
        success: function (data) {
            console.log(data); // This will log the response from the server
        },
        error: function (xhr, status, error) {
            console.error('There was a problem with the AJAX request:', error);
        }
    });
}