class PaginatedTable {
    constructor(tableRef) {
        this.tableRef = tableRef;
        this.tbodyRef = $('tbody', this.tableRef);

        let url = tableRef.data('datasource');
        if (typeof url !== "string")
            throw Error("Datasource attribute was not found for paginated table!");

        this.dataUrl = url;

        let page = tableRef.data('initial-page');
        if (typeof page !== "number" || !page)
            page = 1;

        this.currentPage = page;
        this.totalPages = page;

        let pages = `<ul class="pagination" style="float:right">
    <li class="page-item">
      <a class="page-link" href="?page=` + (page - 1) + `" data-href="` + this.dataUrl + `?page=` + (page - 1) + `" aria-label="Previous">
        <span aria-hidden="true">&laquo;</span>
        <span class="sr-only">Предыдущая</span>
      </a>
    </li>
    <li class="page-item">
      <a class="page-link" href="?page=` + (page + 1) + `" data-href="` + this.dataUrl + `?page=` + (page + 1) + `" aria-label="Next">
        <span aria-hidden="true">&raquo;</span>
        <span class="sr-only">Следующая</span>
      </a>
    </li>
  </ul>`;

        this.topPages = $(pages).insertBefore(this.tableRef);
        this.bottomPages = $(pages).insertAfter(this.tableRef);
        this.pagesRendered = false;
    }

    load() {
        let me = this;

        $.get(me.dataUrl, { page: me.currentPage },
            function (data) {
                if (data) {
                    me.tbodyRef.html(data);
                    if (!me.pagesRendered) {
                        var firstTr = $('tr:first', me.tbodyRef);
                        if (firstTr.length) {
                            me.totalPages = firstTr.data('pages-count');
                            me.renderPageLinks(me.topPages);
                            me.renderPageLinks(me.bottomPages);
                        }

                        me.pagesRendered = true;
                    }
                    $(".popover-handler").popover({ html: true });
                    me.reloadPageLinksState();
                } else {
                    me.tbodyRef.html('<tr><td class="grayed">Нет данных</td></tr>');
                }
            }
        );
    }

    renderPageLinks(pagesRef) {
        var me = this;
        var allPages = '';
        for (var i = 1; i <= me.totalPages; i++) {
            allPages += '<li class="page-item" data-pageid="' + i + '"><a class="page-link" href="?page=' + i + '" data-href="' + me.dataUrl + '?page=' + i + '">' + i + '</a></li>';
        }
        $('li:first', pagesRef).after(allPages);
        pagesRef.click(function (ev) {
            me.onPageClick($(ev.target), this, ev);
        });
    }

    reloadPageLinksState() {
        let me = this;
        $('li:first', me.topPages).toggleClass('disabled', this.currentPage === 1);
        $('li:first', me.bottomPages).toggleClass('disabled', this.currentPage === 1);
        $('li:last', me.topPages).toggleClass('disabled', this.currentPage === this.totalPages);
        $('li:last', me.bottomPages).toggleClass('disabled', this.currentPage === this.totalPages);

        $('li:first', me.topPages).data('pageid', this.currentPage - 1);
        $('li:last', me.topPages).data('pageid', this.currentPage + 1);
        $('li:first', me.bottomPages).data('pageid', this.currentPage - 1);
        $('li:last', me.bottomPages).data('pageid', this.currentPage + 1);

        $('li:first > a', me.topPages).prop('href', 'href="?page=' + (this.currentPage - 1) + '"');
        $('li:first > a', me.bottomPages).prop('href', 'href="?page=' + (this.currentPage - 1) + '"');
        $('li:last > a', me.topPages).prop('href', 'href="?page=' + (this.currentPage + 1) + '"');
        $('li:last > a', me.bottomPages).prop('href', 'href="?page=' + (this.currentPage + 1) + '"');


        $('li', me.topPages).removeClass('active');
        $('li', me.bottomPages).removeClass('active');
        $('li[data-pageid=' + me.currentPage + ']', me.topPages).addClass('active');
        $('li[data-pageid=' + me.currentPage + ']', me.bottomPages).addClass('active');
    }

    onPageClick(sender, context, event) {
        event.preventDefault();
        if (!sender.is('a'))
            sender = sender.closest('a.page-link');

        if(!sender.length)
            return;

        if (sender.hasClass('disabled') || sender.closest('li').hasClass('disabled'))
            return;

        this.currentPage = sender.closest('li').data('pageid');
        this.load();
    }
}



(function () {
    $.fn.getPaginatedTable = function () {
        if ($(this).length !== 1) {
            return undefined;
        }

        return $(this).data('paginatedTable');
    }

    let me = this;

    $("table.table-paginated").each(function () {
        $(this).data('paginatedTable', new PaginatedTable($(this)));
        $(this).getPaginatedTable().load();
    });

    return me;
})();