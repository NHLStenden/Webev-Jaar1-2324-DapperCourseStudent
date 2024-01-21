create table brouwer
(
    brouwcode int  not null
        primary key,
    naam      text null,
    land      text null
);

create table bier
(
    biercode  int    not null
        primary key,
    naam      text   null,
    type      text   null,
    stijl     text   null,
    alcohol   double null,
    brouwcode int    null,
    constraint bier_brouwer_brouwcode_fk
        foreign key (brouwcode) references brouwer (brouwcode)
);

create table kroeg
(
    kroegcode int  not null
        primary key,
    naam      text null,
    adres     text null,
    plaats    text null
);

create table schenkt
(
    kroegcode int not null,
    biercode  int not null,
    primary key (kroegcode, biercode),
    constraint schenkt_bier_biercode_fk
        foreign key (biercode) references bier (biercode),
    constraint schenkt_kroeg_kroegcode_fk
        foreign key (kroegcode) references kroeg (kroegcode)
);

