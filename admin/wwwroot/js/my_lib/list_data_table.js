$(document).ready(function () {
    try {
        //Default data table
        var table = $('#dataTable').DataTable({
            dom: 'lBfrtip',
            bPaginate: false,
            //pageLength: 25,
            //lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]],
            language: {
                "search": "_INPUT_",
                "searchPlaceholder": "Filtrele "
            },
            buttons: [
                {
                    extend: 'pdf',
                    text: 'Pdf',
                    orientation: 'landscape',
                    pageSize: 'LEGAL',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }, {
                    extend: 'excel',
                    text: 'Excel',
                    orientation: 'landscape',
                    pageSize: 'LEGAL',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }, {
                    extend: 'csv',
                    text: 'CSV Çıktısı',
                    orientation: 'landscape',
                    pageSize: 'LEGAL',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }, {
                    extend: 'print',
                    text: 'Yazdır',
                    orientation: 'landscape',
                    pageSize: 'LEGAL',
                    exportOptions: {
                        columns: "thead th:not(.noExport)"
                    }
                }
            ]
        });
        table.buttons().container().appendTo('#dataTable_wrapper .col-md-6:eq(0)');
    } catch (e) {
        console.log(e, "Datatable kütüphanesi bulunamadı.")
    }
});